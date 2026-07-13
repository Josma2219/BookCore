namespace BookCore.ViewModels
{
    public class FavoritoLibroViewModel
    {
        public int FavoritoId { get; set; }

        public int LibroId { get; set; }

        public string Titulo { get; set; }
            = string.Empty;

        public string Autores { get; set; }
            = string.Empty;

        public string Categoria { get; set; }
            = string.Empty;

        public string? ImagenUrl { get; set; }

        public DateTime FechaAgregado { get; set; }

        public int EjemplaresDisponibles { get; set; }
    }
}