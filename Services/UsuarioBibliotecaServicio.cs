using BookCore.Data;
using BookCore.Helpers;
using BookCore.Models.Entidades;
using BookCore.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace BookCore.Services
{
    public class UsuarioBibliotecaServicio : IUsuarioBibliotecaServicio
    {
        private readonly BookCoreContexto _contexto;
        private readonly ILogger<UsuarioBibliotecaServicio> _registro;

        public UsuarioBibliotecaServicio(
            BookCoreContexto contexto,
            ILogger<UsuarioBibliotecaServicio> registro)
        {
            _contexto = contexto;
            _registro = registro;
        }

        public async Task<List<UsuarioBibliotecaViewModel>> ObtenerTodosAsync(
            string? busqueda = null)
        {
            var consulta = _contexto
                .Set<UsuarioBiblioteca>()
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                string textoBusqueda = busqueda.Trim();

                consulta = consulta.Where(usuario =>
                    usuario.Nombre.Contains(textoBusqueda) ||
                    usuario.Apellidos.Contains(textoBusqueda) ||
                    usuario.Cedula.Contains(textoBusqueda) ||
                    (usuario.Correo != null &&
                     usuario.Correo.Contains(textoBusqueda)));
            }

            return await consulta
                .OrderBy(usuario => usuario.Apellidos)
                .ThenBy(usuario => usuario.Nombre)
                .Select(usuario => new UsuarioBibliotecaViewModel
                {
                    UsuarioBibliotecaId = usuario.UsuarioBibliotecaId,
                    Nombre = usuario.Nombre,
                    Apellidos = usuario.Apellidos,
                    Cedula = usuario.Cedula,
                    Correo = usuario.Correo,
                    Telefono = usuario.Telefono,
                    Direccion = usuario.Direccion,
                    Activo = usuario.Activo,
                    FechaRegistro = usuario.FechaRegistro
                })
                .ToListAsync();
        }

        public async Task<List<UsuarioBibliotecaViewModel>> ObtenerActivosAsync()
        {
            return await _contexto
                .Set<UsuarioBiblioteca>()
                .AsNoTracking()
                .Where(usuario => usuario.Activo)
                .OrderBy(usuario => usuario.Apellidos)
                .ThenBy(usuario => usuario.Nombre)
                .Select(usuario => new UsuarioBibliotecaViewModel
                {
                    UsuarioBibliotecaId = usuario.UsuarioBibliotecaId,
                    Nombre = usuario.Nombre,
                    Apellidos = usuario.Apellidos,
                    Cedula = usuario.Cedula,
                    Correo = usuario.Correo,
                    Telefono = usuario.Telefono,
                    Direccion = usuario.Direccion,
                    Activo = usuario.Activo,
                    FechaRegistro = usuario.FechaRegistro
                })
                .ToListAsync();
        }

        public async Task<UsuarioBibliotecaViewModel?> ObtenerPorIdAsync(
            int usuarioBibliotecaId)
        {
            return await _contexto
                .Set<UsuarioBiblioteca>()
                .AsNoTracking()
                .Where(usuario =>
                    usuario.UsuarioBibliotecaId == usuarioBibliotecaId)
                .Select(usuario => new UsuarioBibliotecaViewModel
                {
                    UsuarioBibliotecaId = usuario.UsuarioBibliotecaId,
                    Nombre = usuario.Nombre,
                    Apellidos = usuario.Apellidos,
                    Cedula = usuario.Cedula,
                    Correo = usuario.Correo,
                    Telefono = usuario.Telefono,
                    Direccion = usuario.Direccion,
                    Activo = usuario.Activo,
                    FechaRegistro = usuario.FechaRegistro
                })
                .FirstOrDefaultAsync();
        }

        public async Task<ResultadoOperacion> CrearAsync(
            UsuarioBibliotecaViewModel modelo)
        {
            string nombreLimpio = modelo.Nombre.Trim();
            string apellidosLimpios = modelo.Apellidos.Trim();
            string cedulaLimpia = modelo.Cedula.Trim();

            bool cedulaRepetida = await _contexto
                .Set<UsuarioBiblioteca>()
                .AnyAsync(usuario =>
                    usuario.Cedula == cedulaLimpia);

            if (cedulaRepetida)
            {
                return ResultadoOperacion.Fallido(
                    "Ya existe un usuario con esa cédula o identificación.");
            }

            var usuarioNuevo = new UsuarioBiblioteca
            {
                Nombre = nombreLimpio,
                Apellidos = apellidosLimpios,
                Cedula = cedulaLimpia,
                Correo = LimpiarTextoOpcional(modelo.Correo),
                Telefono = LimpiarTextoOpcional(modelo.Telefono),
                Direccion = LimpiarTextoOpcional(modelo.Direccion),
                Activo = true,
                FechaRegistro = DateTime.Now
            };

            try
            {
                _contexto.Set<UsuarioBiblioteca>().Add(usuarioNuevo);
                await _contexto.SaveChangesAsync();

                return ResultadoOperacion.Correcto(
                    "El usuario se creó correctamente.");
            }
            catch (DbUpdateException excepcion)
            {
                _registro.LogError(
                    excepcion,
                    "Error al crear el usuario de biblioteca {Cedula}.",
                    cedulaLimpia);

                return ResultadoOperacion.Fallido(
                    "No fue posible guardar el usuario.");
            }
        }

        public async Task<ResultadoOperacion> EditarAsync(
            UsuarioBibliotecaViewModel modelo)
        {
            var usuario = await _contexto
                .Set<UsuarioBiblioteca>()
                .FirstOrDefaultAsync(usuarioActual =>
                    usuarioActual.UsuarioBibliotecaId ==
                    modelo.UsuarioBibliotecaId);

            if (usuario is null)
            {
                return ResultadoOperacion.Fallido(
                    "El usuario que intentas editar no existe.");
            }

            string cedulaLimpia = modelo.Cedula.Trim();

            bool cedulaRepetida = await _contexto
                .Set<UsuarioBiblioteca>()
                .AnyAsync(usuarioActual =>
                    usuarioActual.Cedula == cedulaLimpia &&
                    usuarioActual.UsuarioBibliotecaId !=
                    modelo.UsuarioBibliotecaId);

            if (cedulaRepetida)
            {
                return ResultadoOperacion.Fallido(
                    "Ya existe otro usuario con esa cédula o identificación.");
            }

            if (usuario.Activo && !modelo.Activo)
            {
                bool tienePrestamosActivos =
                    await TienePrestamosActivosAsync(
                        modelo.UsuarioBibliotecaId);

                if (tienePrestamosActivos)
                {
                    return ResultadoOperacion.Fallido(
                        "No puedes desactivar este usuario porque tiene préstamos activos.");
                }
            }

            usuario.Nombre = modelo.Nombre.Trim();
            usuario.Apellidos = modelo.Apellidos.Trim();
            usuario.Cedula = cedulaLimpia;
            usuario.Correo = LimpiarTextoOpcional(modelo.Correo);
            usuario.Telefono = LimpiarTextoOpcional(modelo.Telefono);
            usuario.Direccion = LimpiarTextoOpcional(modelo.Direccion);
            usuario.Activo = modelo.Activo;

            try
            {
                await _contexto.SaveChangesAsync();

                return ResultadoOperacion.Correcto(
                    "El usuario se actualizó correctamente.");
            }
            catch (DbUpdateException excepcion)
            {
                _registro.LogError(
                    excepcion,
                    "Error al editar el usuario {UsuarioBibliotecaId}.",
                    modelo.UsuarioBibliotecaId);

                return ResultadoOperacion.Fallido(
                    "No fue posible actualizar el usuario.");
            }
        }

        public async Task<ResultadoOperacion> EliminarAsync(
            int usuarioBibliotecaId)
        {
            var usuario = await _contexto
                .Set<UsuarioBiblioteca>()
                .FirstOrDefaultAsync(usuarioActual =>
                    usuarioActual.UsuarioBibliotecaId ==
                    usuarioBibliotecaId);

            if (usuario is null)
            {
                return ResultadoOperacion.Fallido(
                    "El usuario que intentas eliminar no existe.");
            }

            if (!usuario.Activo)
            {
                return ResultadoOperacion.Fallido(
                    "El usuario ya se encuentra inactivo.");
            }

            bool tienePrestamosActivos =
                await TienePrestamosActivosAsync(usuarioBibliotecaId);

            if (tienePrestamosActivos)
            {
                return ResultadoOperacion.Fallido(
                    "No puedes eliminar este usuario porque tiene préstamos activos.");
            }

            // Lo dejamos en la base para conservar su historial.
            usuario.Activo = false;

            try
            {
                await _contexto.SaveChangesAsync();

                return ResultadoOperacion.Correcto(
                    "El usuario se desactivó correctamente.");
            }
            catch (DbUpdateException excepcion)
            {
                _registro.LogError(
                    excepcion,
                    "Error al desactivar el usuario {UsuarioBibliotecaId}.",
                    usuarioBibliotecaId);

                return ResultadoOperacion.Fallido(
                    "No fue posible desactivar el usuario.");
            }
        }

        private async Task<bool> TienePrestamosActivosAsync(
            int usuarioBibliotecaId)
        {
            return await _contexto
                .Set<Prestamo>()
                .AsNoTracking()
                .AnyAsync(prestamo =>
                    prestamo.UsuarioBibliotecaId ==
                    usuarioBibliotecaId &&
                    (
                        prestamo.Estado == EstadosPrestamo.Activo ||
                        prestamo.Estado == EstadosPrestamo.Vencido
                    ));
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