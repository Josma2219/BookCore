namespace BookCore.ViewModels
{
    public class PanelAdministrativoViewModel
    {
        public int TotalLibrosActivos { get; set; }

        public int TotalUsuariosActivos { get; set; }

        public int TotalEjemplaresActivos { get; set; }

        public int EjemplaresDisponibles { get; set; }

        public int PrestamosPendientes { get; set; }

        public int PrestamosVencidos { get; set; }

        public int PrestamosVencenHoy { get; set; }

        public List<PrestamoListadoViewModel>
            PrestamosRecientes
        { get; set; } = [];
    }
}