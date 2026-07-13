using BookCore.Helpers;
using BookCore.ViewModels;

namespace BookCore.Services
{
    public interface IEjemplarServicio
    {
        Task<EjemplarIndiceViewModel> ObtenerIndiceAsync(
            string? busqueda,
            int? libroId,
            string? estado,
            bool soloActivos);

        Task<EjemplarListadoViewModel?> ObtenerPorIdAsync(
            int ejemplarId);

        Task<EjemplarFormularioViewModel> PrepararCreacionAsync(
            int? libroId = null);

        Task<EjemplarFormularioViewModel?> PrepararEdicionAsync(
            int ejemplarId);

        Task<EjemplarFormularioViewModel> CargarOpcionesAsync(
            EjemplarFormularioViewModel modelo);

        Task<List<EjemplarListadoViewModel>>
            ObtenerDisponiblesPorLibroAsync(int libroId);

        Task<ResultadoOperacion> CrearAsync(
            EjemplarFormularioViewModel modelo);

        Task<ResultadoOperacion> EditarAsync(
            EjemplarFormularioViewModel modelo);

        Task<ResultadoOperacion> EliminarAsync(
            int ejemplarId);
    }
}