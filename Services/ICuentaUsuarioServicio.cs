using BookCore.Helpers;
using BookCore.ViewModels;

namespace BookCore.Services
{
    public interface ICuentaUsuarioServicio
    {
        Task<CuentaUsuarioFormularioViewModel?>
            PrepararAsync(int usuarioBibliotecaId);

        Task<ResultadoOperacion> GuardarAsync(
            CuentaUsuarioFormularioViewModel modelo);
    }
}