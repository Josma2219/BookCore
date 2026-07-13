namespace BookCore.ViewModels
{
    public class CatalogoLibroViewModel
    {
        public int LibroId { get; set; }

        public string Titulo { get; set; } = string.Empty;

        public string Autores { get; set; } = string.Empty;

        public string CategoriaNombre { get; set; } = string.Empty;

        public string? Editorial { get; set; }

        public int? AnioPublicacion { get; set; }

        public string? ImagenUrl { get; set; }

        public string? Descripcion { get; set; }

        public int TotalEjemplares { get; set; }

        public int EjemplaresDisponibles { get; set; }

        public bool Disponible =>
            EjemplaresDisponibles > 0;
    }
}