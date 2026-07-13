namespace BookCore.ViewModels
{
    public class ResultadoAccesoViewModel
    {
        public int IdCuenta { get; set; }

        public int? UsuarioBibliotecaId { get; set; }

        public string NombreUsuario { get; set; }
            = string.Empty;

        public string Correo { get; set; }
            = string.Empty;

        public string Rol { get; set; }
            = string.Empty;
    }
}