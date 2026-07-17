namespace BookCore.ViewModels
{
    public class LibroRankingViewModel
    {
        public int LibroId { get; set; }

        public string Titulo { get; set; } = string.Empty;

        public int TotalPrestamos { get; set; }
    }
}
