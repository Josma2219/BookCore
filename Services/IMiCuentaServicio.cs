using BookCore.ViewModels;

namespace BookCore.Services
{
    public interface IMiCuentaServicio
    {
        Task<MiCuentaViewModel?>
            ObtenerResumenAsync(
                int usuarioBibliotecaId);

        Task<HistorialUsuarioViewModel?>
            ObtenerHistorialAsync(
                int usuarioBibliotecaId);
    }
}