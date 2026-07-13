namespace BookCore.ViewModels
{
    public class EjemplarListadoViewModel
    {
        public int EjemplarId { get; set; }

        public int LibroId { get; set; }

        public string LibroTitulo { get; set; } = string.Empty;

        public string CodigoInterno { get; set; } = string.Empty;

        public string Estado { get; set; } = string.Empty;

        public string? Ubicacion { get; set; }

        public DateTime FechaIngreso { get; set; }

        public bool Activo { get; set; }

        public bool TienePrestamoActivo { get; set; }
    }
}