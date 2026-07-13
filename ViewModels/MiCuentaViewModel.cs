namespace BookCore.ViewModels
{
    public class MiCuentaViewModel
    {
        public int UsuarioBibliotecaId { get; set; }

        public string NombreCompleto { get; set; }
            = string.Empty;

        public string Cedula { get; set; }
            = string.Empty;

        public string? Correo { get; set; }

        public string? Telefono { get; set; }

        public int TotalPrestamosHistoricos { get; set; }

        public int TotalFavoritos { get; set; }

        public List<PrestamoUsuarioViewModel>
            PrestamosActuales
        { get; set; } = [];
    }
}