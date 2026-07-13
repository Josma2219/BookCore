using BookCore.Data;
using BookCore.Models.Entidades;
using BookCore.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookCore.Services
{
    public class AccesoServicio : IAccesoServicio
    {
        private readonly BookCoreContexto _contexto;

        private readonly
            IPasswordHasher<UsuarioAdministrativo>
            _generadorAdministrador;

        private readonly
            IPasswordHasher<CuentaUsuario>
            _generadorUsuario;

        public AccesoServicio(
            BookCoreContexto contexto,
            IPasswordHasher<UsuarioAdministrativo>
                generadorAdministrador,
            IPasswordHasher<CuentaUsuario>
                generadorUsuario)
        {
            _contexto = contexto;

            _generadorAdministrador =
                generadorAdministrador;

            _generadorUsuario =
                generadorUsuario;
        }

        public async Task<ResultadoAccesoViewModel?>
            ValidarCredencialesAsync(
                string identificador,
                string contrasena)
        {
            string identificadorLimpio =
                identificador.Trim();

            var administrador = await _contexto
                .Set<UsuarioAdministrativo>()
                .FirstOrDefaultAsync(usuario =>
                    usuario.Activo &&
                    (
                        usuario.NombreUsuario ==
                        identificadorLimpio ||

                        usuario.Correo ==
                        identificadorLimpio
                    ));

            if (administrador is not null)
            {
                var resultadoAdministrador =
                    _generadorAdministrador
                        .VerifyHashedPassword(
                            administrador,
                            administrador.ContrasenaHash,
                            contrasena);

                if (resultadoAdministrador !=
                    PasswordVerificationResult.Failed)
                {
                    if (resultadoAdministrador ==
                        PasswordVerificationResult
                            .SuccessRehashNeeded)
                    {
                        administrador.ContrasenaHash =
                            _generadorAdministrador
                                .HashPassword(
                                    administrador,
                                    contrasena);

                        await _contexto.SaveChangesAsync();
                    }

                    return new ResultadoAccesoViewModel
                    {
                        IdCuenta =
                            administrador
                                .UsuarioAdministrativoId,

                        NombreUsuario =
                            administrador.NombreUsuario,

                        Correo =
                            administrador.Correo,

                        Rol =
                            "Administrador"
                    };
                }
            }

            var resultadoCuenta = await (
                from cuenta in _contexto
                    .Set<CuentaUsuario>()

                join usuario in _contexto
                    .Set<UsuarioBiblioteca>()
                    on cuenta.UsuarioBibliotecaId
                    equals usuario.UsuarioBibliotecaId

                where cuenta.Activo &&
                      usuario.Activo &&
                      (
                          cuenta.NombreUsuario ==
                          identificadorLimpio ||

                          cuenta.Correo ==
                          identificadorLimpio
                      )

                select new
                {
                    Cuenta = cuenta,
                    Usuario = usuario
                })
                .FirstOrDefaultAsync();

            if (resultadoCuenta is null)
            {
                return null;
            }

            var resultadoContrasena =
                _generadorUsuario
                    .VerifyHashedPassword(
                        resultadoCuenta.Cuenta,
                        resultadoCuenta.Cuenta
                            .ContrasenaHash,
                        contrasena);

            if (resultadoContrasena ==
                PasswordVerificationResult.Failed)
            {
                return null;
            }

            if (resultadoContrasena ==
                PasswordVerificationResult
                    .SuccessRehashNeeded)
            {
                resultadoCuenta.Cuenta
                    .ContrasenaHash =
                    _generadorUsuario
                        .HashPassword(
                            resultadoCuenta.Cuenta,
                            contrasena);

                await _contexto.SaveChangesAsync();
            }

            return new ResultadoAccesoViewModel
            {
                IdCuenta =
                    resultadoCuenta.Cuenta
                        .CuentaUsuarioId,

                UsuarioBibliotecaId =
                    resultadoCuenta.Usuario
                        .UsuarioBibliotecaId,

                NombreUsuario =
                    resultadoCuenta.Cuenta
                        .NombreUsuario,

                Correo =
                    resultadoCuenta.Cuenta.Correo,

                Rol =
                    "Usuario"
            };
        }
    }
}