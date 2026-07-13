using BookCore.Data;
using BookCore.Helpers;
using BookCore.Models.Entidades;
using BookCore.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace BookCore.Services
{
    public class CategoriaServicio : ICategoriaServicio
    {
        private readonly BookCoreContexto _contexto;
        private readonly ILogger<CategoriaServicio> _registro;

        public CategoriaServicio(
            BookCoreContexto contexto,
            ILogger<CategoriaServicio> registro)
        {
            _contexto = contexto;
            _registro = registro;
        }

        public async Task<List<CategoriaViewModel>> ObtenerTodasAsync()
        {
            return await _contexto
                .Set<Categoria>()
                .AsNoTracking()
                .OrderBy(categoria => categoria.Nombre)
                .Select(categoria => new CategoriaViewModel
                {
                    CategoriaId = categoria.CategoriaId,
                    Nombre = categoria.Nombre,
                    Descripcion = categoria.Descripcion,
                    Activo = categoria.Activo,
                    FechaCreacion = categoria.FechaCreacion
                })
                .ToListAsync();
        }

        public async Task<CategoriaViewModel?> ObtenerPorIdAsync(
            int categoriaId)
        {
            return await _contexto
                .Set<Categoria>()
                .AsNoTracking()
                .Where(categoria =>
                    categoria.CategoriaId == categoriaId)
                .Select(categoria => new CategoriaViewModel
                {
                    CategoriaId = categoria.CategoriaId,
                    Nombre = categoria.Nombre,
                    Descripcion = categoria.Descripcion,
                    Activo = categoria.Activo,
                    FechaCreacion = categoria.FechaCreacion
                })
                .FirstOrDefaultAsync();
        }

        public async Task<ResultadoOperacion> CrearAsync(
            CategoriaViewModel modelo)
        {
            string nombreLimpio = modelo.Nombre.Trim();

            bool nombreRepetido = await _contexto
                .Set<Categoria>()
                .AnyAsync(categoria =>
                    categoria.Nombre == nombreLimpio);

            if (nombreRepetido)
            {
                return ResultadoOperacion.Fallido(
                    "Ya existe una categoría con ese nombre.");
            }

            var categoriaNueva = new Categoria
            {
                Nombre = nombreLimpio,
                Descripcion = LimpiarTextoOpcional(
                    modelo.Descripcion),
                Activo = true,
                FechaCreacion = DateTime.Now
            };

            try
            {
                _contexto.Set<Categoria>().Add(categoriaNueva);
                await _contexto.SaveChangesAsync();

                return ResultadoOperacion.Correcto(
                    "La categoría se creó correctamente.");
            }
            catch (DbUpdateException excepcion)
            {
                _registro.LogError(
                    excepcion,
                    "Ocurrió un error al crear la categoría {NombreCategoria}.",
                    nombreLimpio);

                return ResultadoOperacion.Fallido(
                    "No fue posible guardar la categoría.");
            }
        }

        public async Task<ResultadoOperacion> EditarAsync(
            CategoriaViewModel modelo)
        {
            var categoria = await _contexto
                .Set<Categoria>()
                .FirstOrDefaultAsync(categoriaActual =>
                    categoriaActual.CategoriaId ==
                    modelo.CategoriaId);

            if (categoria is null)
            {
                return ResultadoOperacion.Fallido(
                    "La categoría que intentas editar no existe.");
            }

            string nombreLimpio = modelo.Nombre.Trim();

            bool nombreRepetido = await _contexto
                .Set<Categoria>()
                .AnyAsync(categoriaActual =>
                    categoriaActual.Nombre == nombreLimpio &&
                    categoriaActual.CategoriaId != modelo.CategoriaId);

            if (nombreRepetido)
            {
                return ResultadoOperacion.Fallido(
                    "Ya existe otra categoría con ese nombre.");
            }

            // No dejamos apagar una categoría que todavía tiene libros activos.
            if (categoria.Activo && !modelo.Activo)
            {
                bool tieneLibrosActivos = await _contexto
                    .Set<Libro>()
                    .AnyAsync(libro =>
                        libro.CategoriaId == modelo.CategoriaId &&
                        libro.Activo);

                if (tieneLibrosActivos)
                {
                    return ResultadoOperacion.Fallido(
                        "No puedes desactivar esta categoría porque tiene libros activos.");
                }
            }

            categoria.Nombre = nombreLimpio;
            categoria.Descripcion = LimpiarTextoOpcional(
                modelo.Descripcion);
            categoria.Activo = modelo.Activo;

            try
            {
                await _contexto.SaveChangesAsync();

                return ResultadoOperacion.Correcto(
                    "La categoría se actualizó correctamente.");
            }
            catch (DbUpdateException excepcion)
            {
                _registro.LogError(
                    excepcion,
                    "Ocurrió un error al editar la categoría {CategoriaId}.",
                    modelo.CategoriaId);

                return ResultadoOperacion.Fallido(
                    "No fue posible actualizar la categoría.");
            }
        }

        public async Task<ResultadoOperacion> EliminarAsync(
            int categoriaId)
        {
            var categoria = await _contexto
                .Set<Categoria>()
                .FirstOrDefaultAsync(categoriaActual =>
                    categoriaActual.CategoriaId == categoriaId);

            if (categoria is null)
            {
                return ResultadoOperacion.Fallido(
                    "La categoría que intentas eliminar no existe.");
            }

            if (!categoria.Activo)
            {
                return ResultadoOperacion.Fallido(
                    "La categoría ya se encuentra inactiva.");
            }

            bool tieneLibrosActivos = await _contexto
                .Set<Libro>()
                .AnyAsync(libro =>
                    libro.CategoriaId == categoriaId &&
                    libro.Activo);

            if (tieneLibrosActivos)
            {
                return ResultadoOperacion.Fallido(
                    "No puedes eliminar esta categoría porque tiene libros activos.");
            }

            // No borramos el registro para no perder información histórica.
            categoria.Activo = false;

            try
            {
                await _contexto.SaveChangesAsync();

                return ResultadoOperacion.Correcto(
                    "La categoría se desactivó correctamente.");
            }
            catch (DbUpdateException excepcion)
            {
                _registro.LogError(
                    excepcion,
                    "Ocurrió un error al desactivar la categoría {CategoriaId}.",
                    categoriaId);

                return ResultadoOperacion.Fallido(
                    "No fue posible desactivar la categoría.");
            }
        }

        private static string? LimpiarTextoOpcional(string? texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
            {
                return null;
            }

            return texto.Trim();
        }
    }
}