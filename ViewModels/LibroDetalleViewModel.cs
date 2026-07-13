namespace BookCore.ViewModels
{
    public class LibroDetalleViewModel
    {
        public int LibroId { get; set; }

        public string Titulo { get; set; } = string.Empty;

        public string? Isbn { get; set; }

        public string? Editorial { get; set; }

        public int? AnioPublicacion { get; set; }

        public string? Descripcion { get; set; }

        public string CategoriaNombre { get; set; } = string.Empty;

        public string Autores { get; set; } = string.Empty;

        public string? ImagenUrl { get; set; }

        public bool Activo { get; set; }

        public DateTime FechaRegistro { get; set; }

        public int TotalEjemplares { get; set; }

        public int EjemplaresDisponibles { get; set; }

        public int EjemplaresPrestados { get; set; }

        public int EjemplaresDanados { get; set; }
    }
}