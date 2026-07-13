using BookCore.Models.Entidades;

namespace BookCore.Services
{
    public interface IAccesoServicio
    {
        Task<UsuarioAdministrativo?> ValidarCredencialesAsync(
            string identificador,
            string contrasena);
    }
}