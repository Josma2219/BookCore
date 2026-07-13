namespace BookCore.ViewModels
{
    public class HistorialUsuarioViewModel
    {
        public string NombreCompleto { get; set; }
            = string.Empty;

        public List<PrestamoUsuarioViewModel>
            Prestamos
        { get; set; } = [];
    }
}