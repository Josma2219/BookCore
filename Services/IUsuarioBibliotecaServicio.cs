using BookCore.Helpers;
using BookCore.ViewModels;

namespace BookCore.Services
{
    public interface IUsuarioBibliotecaServicio
    {
        Task<List<UsuarioBibliotecaViewModel>> ObtenerTodosAsync(
            string? busqueda = null);

        // Este método se usará luego en el formulario de préstamos.
        Task<List<UsuarioBibliotecaViewModel>> ObtenerActivosAsync();

        Task<UsuarioBibliotecaViewModel?> ObtenerPorIdAsync(
            int usuarioBibliotecaId);

        Task<ResultadoOperacion> CrearAsync(
            UsuarioBibliotecaViewModel modelo);

        Task<ResultadoOperacion> EditarAsync(
            UsuarioBibliotecaViewModel modelo);

        Task<ResultadoOperacion> EliminarAsync(
            int usuarioBibliotecaId);
    }
}