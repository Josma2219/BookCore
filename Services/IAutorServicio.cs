using BookCore.Helpers;
using BookCore.ViewModels;

namespace BookCore.Services
{
    public interface IAutorServicio
    {
        Task<List<AutorViewModel>> ObtenerTodosAsync();

        Task<AutorViewModel?> ObtenerPorIdAsync(int autorId);

        Task<ResultadoOperacion> CrearAsync(
            AutorViewModel modelo);

        Task<ResultadoOperacion> EditarAsync(
            AutorViewModel modelo);

        Task<ResultadoOperacion> EliminarAsync(
            int autorId);
    }
}