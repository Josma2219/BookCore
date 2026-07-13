using BookCore.Data;
using BookCore.Models.Entidades;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookCore.Services
{
    public class AccesoServicio : IAccesoServicio
    {
        private readonly BookCoreContexto _contexto;

        private readonly IPasswordHasher<UsuarioAdministrativo>
            _generadorContrasenas;

        private readonly ILogger<AccesoServicio> _registro;

        public AccesoServicio(
            BookCoreContexto contexto,
            IPasswordHasher<UsuarioAdministrativo> generadorContrasenas,
            ILogger<AccesoServicio> registro)
        {
            _contexto = contexto;
            _generadorContrasenas = generadorContrasenas;
            _registro = registro;
        }

        public async Task<UsuarioAdministrativo?>
            ValidarCredencialesAsync(
                string identificador,
                string contrasena)
        {
            string identificadorLimpio = identificador.Trim();

            var usuario = await _contexto
                .Set<UsuarioAdministrativo>()
                .FirstOrDefaultAsync(usuarioActual =>
                    usuarioActual.Activo &&
                    (
                        usuarioActual.NombreUsuario ==
                        identificadorLimpio ||

                        usuarioActual.Correo ==
                        identificadorLimpio
                    ));

            if (usuario is null)
            {
                return null;
            }

            var resultado = _generadorContrasenas
                .VerifyHashedPassword(
                    usuario,
                    usuario.ContrasenaHash,
                    contrasena);

            if (resultado == PasswordVerificationResult.Failed)
            {
                return null;
            }

            // Si el sistema detecta un formato antiguo,
            // vuelve a guardar la contraseña con el formato actual.
            if (resultado ==
                PasswordVerificationResult.SuccessRehashNeeded)
            {
                usuario.ContrasenaHash = _generadorContrasenas
                    .HashPassword(usuario, contrasena);

                await _contexto.SaveChangesAsync();

                _registro.LogInformation(
                    "Se actualizó el hash del usuario {UsuarioId}.",
                    usuario.UsuarioAdministrativoId);
            }

            return usuario;
        }
    }
}