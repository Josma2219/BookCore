using BookCore.Helpers;
using BookCore.ViewModels;

namespace BookCore.Services
{
    public interface IPrestamoServicio
    {
        Task<PrestamoIndiceViewModel> ObtenerIndiceAsync(
            string? busqueda,
            string? estado,
            DateTime? fechaDesde,
            DateTime? fechaHasta);

        Task<PrestamoDetalleViewModel?> ObtenerDetalleAsync(
            int prestamoId);

        Task<PrestamoFormularioViewModel> PrepararCreacionAsync(
            int? usuarioBibliotecaId = null,
            int? libroId = null);

        Task<PrestamoFormularioViewModel> CargarOpcionesAsync(
            PrestamoFormularioViewModel modelo);

        Task<List<OpcionEjemplarViewModel>>
            ObtenerEjemplaresDisponiblesAsync(int libroId);

        Task<ResultadoOperacion> CrearAsync(
            PrestamoFormularioViewModel modelo);

        Task<ResultadoOperacion> DevolverAsync(
            int prestamoId);
    }
}