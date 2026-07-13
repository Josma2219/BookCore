using BookCore.ViewModels;

namespace BookCore.Services
{
    public interface ICatalogoServicio
    {
        Task<CatalogoIndiceViewModel> ObtenerIndiceAsync(
            string? busqueda,
            int? categoriaId);

        Task<LibroDetalleViewModel?> ObtenerDetalleAsync(
            int libroId);
    }
}