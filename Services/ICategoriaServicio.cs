using BookCore.Helpers;
using BookCore.ViewModels;

namespace BookCore.Services
{
    public interface ICategoriaServicio
    {
        Task<List<CategoriaViewModel>> ObtenerTodasAsync();

        Task<CategoriaViewModel?> ObtenerPorIdAsync(int categoriaId);

        Task<ResultadoOperacion> CrearAsync(
            CategoriaViewModel modelo);

        Task<ResultadoOperacion> EditarAsync(
            CategoriaViewModel modelo);

        Task<ResultadoOperacion> EliminarAsync(
            int categoriaId);
    }
}