using BookCore.ViewModels;

namespace BookCore.Services
{
    public interface IPanelAdministrativoServicio
    {
        Task<PanelAdministrativoViewModel>
            ObtenerPanelAsync();
    }
}