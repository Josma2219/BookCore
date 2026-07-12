using BookCore.Helpers;
using BookCore.ViewModels;

namespace BookCore.Services
{
    public interface ILibroServicio
    {
        Task<LibroIndiceViewModel> ObtenerIndiceAsync(
            string? busqueda,
            int? categoriaId,
            bool soloActivos);

        Task<LibroDetalleViewModel?> ObtenerDetalleAsync(
            int libroId);

        Task<LibroFormularioViewModel> PrepararCreacionAsync();

        Task<LibroFormularioViewModel?> PrepararEdicionAsync(
            int libroId);

        Task<LibroFormularioViewModel> CargarOpcionesAsync(
            LibroFormularioViewModel modelo);

        Task<ResultadoOperacion> CrearAsync(
            LibroFormularioViewModel modelo);

        Task<ResultadoOperacion> EditarAsync(
            LibroFormularioViewModel modelo);

        Task<ResultadoOperacion> EliminarAsync(
            int libroId);
    }
}