using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookCore.ViewModels
{
    public class LibroIndiceViewModel
    {
        public string? Busqueda { get; set; }

        public int? CategoriaId { get; set; }

        public bool SoloActivos { get; set; }

        public List<SelectListItem> Categorias { get; set; } = [];

        public List<LibroListadoViewModel> Libros { get; set; } = [];
    }
}