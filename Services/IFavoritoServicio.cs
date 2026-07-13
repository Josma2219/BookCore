using BookCore.Helpers;
using BookCore.ViewModels;

namespace BookCore.Services
{
    public interface IFavoritoServicio
    {
        Task<List<FavoritoLibroViewModel>>
            ObtenerPorUsuarioAsync(
                int usuarioBibliotecaId);

        Task<List<int>> ObtenerIdsAsync(
            int usuarioBibliotecaId);

        Task<ResultadoOperacion> AgregarAsync(
            int usuarioBibliotecaId,
            int libroId);

        Task<ResultadoOperacion> EliminarAsync(
            int usuarioBibliotecaId,
            int libroId);
    }
}