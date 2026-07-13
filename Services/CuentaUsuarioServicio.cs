using BookCore.Data;
using BookCore.Helpers;
using BookCore.Models.Entidades;
using BookCore.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookCore.Services
{
    public class CuentaUsuarioServicio
        : ICuentaUsuarioServicio
    {
        private readonly BookCoreContexto _contexto;

        private readonly IPasswordHasher<CuentaUsuario>
            _generadorContrasenas;

        private readonly ILogger<CuentaUsuarioServicio>
            _registro;

        public CuentaUsuarioServicio(
            BookCoreContexto contexto,
            IPasswordHasher<CuentaUsuario> generadorContrasenas,
            ILogger<CuentaUsuarioServicio> registro)
        {
            _contexto = contexto;
            _generadorContrasenas = generadorContrasenas;
            _registro = registro;
        }

        public async Task<CuentaUsuarioFormularioViewModel?>
            PrepararAsync(int usuarioBibliotecaId)
        {
            var usuario = await _contexto
                .Set<UsuarioBiblioteca>()
                .AsNoTracking()
                .FirstOrDefaultAsync(usuarioActual =>
                    usuarioActual.UsuarioBibliotecaId ==
                    usuarioBibliotecaId);

            if (usuario is null)
            {
                return null;
            }

            var cuenta = await _contexto
                .Set<CuentaUsuario>()
                .AsNoTracking()
                .FirstOrDefaultAsync(cuentaActual =>
                    cuentaActual.UsuarioBibliotecaId ==
                    usuarioBibliotecaId);

            return new CuentaUsuarioFormularioViewModel
            {
                CuentaUsuarioId =
                    cuenta?.CuentaUsuarioId ?? 0,

                UsuarioBibliotecaId =
                    usuario.UsuarioBibliotecaId,

                UsuarioNombreCompleto =
                    usuario.Nombre + " " +
                    usuario.Apellidos,

                TieneCuenta = cuenta is not null,

                NombreUsuario =
                    cuenta?.NombreUsuario ?? string.Empty,

                Correo =
                    cuenta?.Correo ??
                    usuario.Correo ??
                    string.Empty,

                Activo =
                    cuenta?.Activo ??
                    usuario.Activo
            };
        }

        public async Task<ResultadoOperacion> GuardarAsync(
            CuentaUsuarioFormularioViewModel modelo)
        {
            var usuario = await _contexto
                .Set<UsuarioBiblioteca>()
                .FirstOrDefaultAsync(usuarioActual =>
                    usuarioActual.UsuarioBibliotecaId ==
                    modelo.UsuarioBibliotecaId);

            if (usuario is null)
            {
                return ResultadoOperacion.Fallido(
                    "El usuario de biblioteca no existe.");
            }

            if (!usuario.Activo && modelo.Activo)
            {
                return ResultadoOperacion.Fallido(
                    "No puedes activar una cuenta cuyo usuario está inactivo.");
            }

            string nombreUsuarioLimpio =
                modelo.NombreUsuario.Trim();

            string correoLimpio =
                modelo.Correo.Trim();

            var cuenta = await _contexto
                .Set<CuentaUsuario>()
                .FirstOrDefaultAsync(cuentaActual =>
                    cuentaActual.UsuarioBibliotecaId ==
                    modelo.UsuarioBibliotecaId);

            int cuentaActualId =
                cuenta?.CuentaUsuarioId ?? 0;

            bool nombreRepetido = await _contexto
                .Set<CuentaUsuario>()
                .AnyAsync(cuentaActual =>
                    cuentaActual.NombreUsuario ==
                    nombreUsuarioLimpio &&
                    cuentaActual.CuentaUsuarioId !=
                    cuentaActualId);

            if (nombreRepetido)
            {
                return ResultadoOperacion.Fallido(
                    "Ya existe una cuenta con ese nombre de usuario.");
            }

            bool correoRepetido = await _contexto
                .Set<CuentaUsuario>()
                .AnyAsync(cuentaActual =>
                    cuentaActual.Correo ==
                    correoLimpio &&
                    cuentaActual.CuentaUsuarioId !=
                    cuentaActualId);

            if (correoRepetido)
            {
                return ResultadoOperacion.Fallido(
                    "Ya existe una cuenta con ese correo.");
            }

            if (cuenta is null &&
                string.IsNullOrWhiteSpace(
                    modelo.ContrasenaNueva))
            {
                return ResultadoOperacion.Fallido(
                    "Debes indicar una contraseña para crear la cuenta.");
            }

            try
            {
                if (cuenta is null)
                {
                    cuenta = new CuentaUsuario
                    {
                        UsuarioBibliotecaId =
                            modelo.UsuarioBibliotecaId,

                        NombreUsuario =
                            nombreUsuarioLimpio,

                        Correo =
                            correoLimpio,

                        ContrasenaHash =
                            string.Empty,

                        Activo =
                            modelo.Activo,

                        FechaCreacion =
                            DateTime.Now
                    };

                    cuenta.ContrasenaHash =
                        _generadorContrasenas
                            .HashPassword(
                                cuenta,
                                modelo.ContrasenaNueva!);

                    _contexto
                        .Set<CuentaUsuario>()
                        .Add(cuenta);

                    await _contexto.SaveChangesAsync();

                    return ResultadoOperacion.Correcto(
                        "La cuenta del usuario se creó correctamente.");
                }

                cuenta.NombreUsuario =
                    nombreUsuarioLimpio;

                cuenta.Correo =
                    correoLimpio;

                cuenta.Activo =
                    modelo.Activo;

                if (!string.IsNullOrWhiteSpace(
                    modelo.ContrasenaNueva))
                {
                    cuenta.ContrasenaHash =
                        _generadorContrasenas
                            .HashPassword(
                                cuenta,
                                modelo.ContrasenaNueva);
                }

                await _contexto.SaveChangesAsync();

                return ResultadoOperacion.Correcto(
                    "La cuenta del usuario se actualizó correctamente.");
            }
            catch (DbUpdateException excepcion)
            {
                _registro.LogError(
                    excepcion,
                    "Error al configurar la cuenta del usuario {UsuarioBibliotecaId}.",
                    modelo.UsuarioBibliotecaId);

                return ResultadoOperacion.Fallido(
                    "No fue posible guardar la cuenta.");
            }
        }
    }
}