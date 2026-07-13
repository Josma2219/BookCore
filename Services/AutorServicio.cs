using BookCore.Data;
using BookCore.Helpers;
using BookCore.Models.Entidades;
using BookCore.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace BookCore.Services
{
    public class AutorServicio : IAutorServicio
    {
        private readonly BookCoreContexto _contexto;
        private readonly ILogger<AutorServicio> _registro;

        public AutorServicio(
            BookCoreContexto contexto,
            ILogger<AutorServicio> registro)
        {
            _contexto = contexto;
            _registro = registro;
        }

        public async Task<List<AutorViewModel>> ObtenerTodosAsync()
        {
            return await _contexto
                .Set<Autor>()
                .AsNoTracking()
                .OrderBy(autor => autor.Apellidos)
                .ThenBy(autor => autor.Nombre)
                .Select(autor => new AutorViewModel
                {
                    AutorId = autor.AutorId,
                    Nombre = autor.Nombre,
                    Apellidos = autor.Apellidos,
                    Nacionalidad = autor.Nacionalidad,
                    FechaNacimiento = autor.FechaNacimiento,
                    Biografia = autor.Biografia,
                    Activo = autor.Activo,
                    FechaCreacion = autor.FechaCreacion
                })
                .ToListAsync();
        }

        public async Task<AutorViewModel?> ObtenerPorIdAsync(
            int autorId)
        {
            return await _contexto
                .Set<Autor>()
                .AsNoTracking()
                .Where(autor => autor.AutorId == autorId)
                .Select(autor => new AutorViewModel
                {
                    AutorId = autor.AutorId,
                    Nombre = autor.Nombre,
                    Apellidos = autor.Apellidos,
                    Nacionalidad = autor.Nacionalidad,
                    FechaNacimiento = autor.FechaNacimiento,
                    Biografia = autor.Biografia,
                    Activo = autor.Activo,
                    FechaCreacion = autor.FechaCreacion
                })
                .FirstOrDefaultAsync();
        }

        public async Task<ResultadoOperacion> CrearAsync(
            AutorViewModel modelo)
        {
            string nombreLimpio = modelo.Nombre.Trim();
            string apellidosLimpios = modelo.Apellidos.Trim();

            if (FechaEsFutura(modelo.FechaNacimiento))
            {
                return ResultadoOperacion.Fallido(
                    "La fecha de nacimiento no puede ser una fecha futura.");
            }

            bool autorRepetido = await _contexto
                .Set<Autor>()
                .AnyAsync(autor =>
                    autor.Nombre == nombreLimpio &&
                    autor.Apellidos == apellidosLimpios);

            if (autorRepetido)
            {
                return ResultadoOperacion.Fallido(
                    "Ya existe un autor con ese nombre y esos apellidos.");
            }

            var autorNuevo = new Autor
            {
                Nombre = nombreLimpio,
                Apellidos = apellidosLimpios,
                Nacionalidad = LimpiarTextoOpcional(
                    modelo.Nacionalidad),
                FechaNacimiento = modelo.FechaNacimiento,
                Biografia = LimpiarTextoOpcional(
                    modelo.Biografia),
                Activo = true,
                FechaCreacion = DateTime.Now
            };

            try
            {
                _contexto.Set<Autor>().Add(autorNuevo);
                await _contexto.SaveChangesAsync();

                return ResultadoOperacion.Correcto(
                    "El autor se creó correctamente.");
            }
            catch (DbUpdateException excepcion)
            {
                _registro.LogError(
                    excepcion,
                    "Ocurrió un error al crear el autor {Nombre} {Apellidos}.",
                    nombreLimpio,
                    apellidosLimpios);

                return ResultadoOperacion.Fallido(
                    "No fue posible guardar el autor.");
            }
        }

        public async Task<ResultadoOperacion> EditarAsync(
            AutorViewModel modelo)
        {
            var autor = await _contexto
                .Set<Autor>()
                .FirstOrDefaultAsync(autorActual =>
                    autorActual.AutorId == modelo.AutorId);

            if (autor is null)
            {
                return ResultadoOperacion.Fallido(
                    "El autor que intentas editar no existe.");
            }

            string nombreLimpio = modelo.Nombre.Trim();
            string apellidosLimpios = modelo.Apellidos.Trim();

            if (FechaEsFutura(modelo.FechaNacimiento))
            {
                return ResultadoOperacion.Fallido(
                    "La fecha de nacimiento no puede ser una fecha futura.");
            }

            bool autorRepetido = await _contexto
                .Set<Autor>()
                .AnyAsync(autorActual =>
                    autorActual.Nombre == nombreLimpio &&
                    autorActual.Apellidos == apellidosLimpios &&
                    autorActual.AutorId != modelo.AutorId);

            if (autorRepetido)
            {
                return ResultadoOperacion.Fallido(
                    "Ya existe otro autor con ese nombre y esos apellidos.");
            }

            if (autor.Activo && !modelo.Activo)
            {
                bool tieneLibrosActivos =
                    await TieneLibrosActivosAsync(modelo.AutorId);

                if (tieneLibrosActivos)
                {
                    return ResultadoOperacion.Fallido(
                        "No puedes desactivar este autor porque tiene libros activos.");
                }
            }

            autor.Nombre = nombreLimpio;
            autor.Apellidos = apellidosLimpios;
            autor.Nacionalidad = LimpiarTextoOpcional(
                modelo.Nacionalidad);
            autor.FechaNacimiento = modelo.FechaNacimiento;
            autor.Biografia = LimpiarTextoOpcional(
                modelo.Biografia);
            autor.Activo = modelo.Activo;

            try
            {
                await _contexto.SaveChangesAsync();

                return ResultadoOperacion.Correcto(
                    "El autor se actualizó correctamente.");
            }
            catch (DbUpdateException excepcion)
            {
                _registro.LogError(
                    excepcion,
                    "Ocurrió un error al editar el autor {AutorId}.",
                    modelo.AutorId);

                return ResultadoOperacion.Fallido(
                    "No fue posible actualizar el autor.");
            }
        }

        public async Task<ResultadoOperacion> EliminarAsync(
            int autorId)
        {
            var autor = await _contexto
                .Set<Autor>()
                .FirstOrDefaultAsync(autorActual =>
                    autorActual.AutorId == autorId);

            if (autor is null)
            {
                return ResultadoOperacion.Fallido(
                    "El autor que intentas eliminar no existe.");
            }

            if (!autor.Activo)
            {
                return ResultadoOperacion.Fallido(
                    "El autor ya se encuentra inactivo.");
            }

            bool tieneLibrosActivos =
                await TieneLibrosActivosAsync(autorId);

            if (tieneLibrosActivos)
            {
                return ResultadoOperacion.Fallido(
                    "No puedes eliminar este autor porque tiene libros activos.");
            }

            // Lo dejamos inactivo para no perder relaciones ni historial.
            autor.Activo = false;

            try
            {
                await _contexto.SaveChangesAsync();

                return ResultadoOperacion.Correcto(
                    "El autor se desactivó correctamente.");
            }
            catch (DbUpdateException excepcion)
            {
                _registro.LogError(
                    excepcion,
                    "Ocurrió un error al desactivar el autor {AutorId}.",
                    autorId);

                return ResultadoOperacion.Fallido(
                    "No fue posible desactivar el autor.");
            }
        }

        private async Task<bool> TieneLibrosActivosAsync(
    int autorId)
        {
            // Entity Framework maneja LibroAutor como una relación automática.
            // Por eso revisamos los libros directamente desde el autor.
            return await _contexto
                .Set<Autor>()
                .AsNoTracking()
                .Where(autor => autor.AutorId == autorId)
                .SelectMany(autor => autor.Libro)
                .AnyAsync(libro => libro.Activo);
        }

        private static bool FechaEsFutura(
            DateOnly? fechaNacimiento)
        {
            if (!fechaNacimiento.HasValue)
            {
                return false;
            }

            DateOnly fechaActual =
                DateOnly.FromDateTime(DateTime.Today);

            return fechaNacimiento.Value > fechaActual;
        }

        private static string? LimpiarTextoOpcional(
            string? texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
            {
                return null;
            }

            return texto.Trim();
        }
    }
}