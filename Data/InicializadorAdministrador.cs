using BookCore.Models.Entidades;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookCore.Data
{
    public static class InicializadorAdministrador
    {
        public static async Task CrearAsync(
            IServiceProvider servicios)
        {
            using var alcance = servicios.CreateScope();

            var contexto = alcance.ServiceProvider
                .GetRequiredService<BookCoreContexto>();

            var configuracion = alcance.ServiceProvider
                .GetRequiredService<IConfiguration>();

            var generadorContrasenas = alcance.ServiceProvider
                .GetRequiredService<
                    IPasswordHasher<UsuarioAdministrativo>>();

            string nombreUsuario =
                configuracion[
                    "AdministradorInicial:NombreUsuario"]
                ?? string.Empty;

            string correo =
                configuracion[
                    "AdministradorInicial:Correo"]
                ?? string.Empty;

            string contrasena =
                configuracion[
                    "AdministradorInicial:Contrasena"]
                ?? string.Empty;

            if (string.IsNullOrWhiteSpace(nombreUsuario) ||
                string.IsNullOrWhiteSpace(correo) ||
                string.IsNullOrWhiteSpace(contrasena))
            {
                throw new InvalidOperationException(
                    "Debes configurar AdministradorInicial " +
                    "en los secretos de usuario.");
            }

            bool administradorExiste = await contexto
                .Set<UsuarioAdministrativo>()
                .AnyAsync(usuario =>
                    usuario.NombreUsuario == nombreUsuario ||
                    usuario.Correo == correo);

            if (administradorExiste)
            {
                return;
            }

            var administrador = new UsuarioAdministrativo
            {
                NombreUsuario = nombreUsuario.Trim(),
                Correo = correo.Trim(),
                ContrasenaHash = string.Empty,
                Rol = "Administrador",
                Activo = true,
                FechaCreacion = DateTime.Now
            };

            administrador.ContrasenaHash =
                generadorContrasenas.HashPassword(
                    administrador,
                    contrasena);

            contexto
                .Set<UsuarioAdministrativo>()
                .Add(administrador);

            await contexto.SaveChangesAsync();
        }
    }
}