using BookCore.ViewModels;

namespace BookCore.Services
{
    public interface IAccesoServicio
    {
        Task<ResultadoAccesoViewModel?>
            ValidarCredencialesAsync(
                string identificador,
                string contrasena);
    }
}