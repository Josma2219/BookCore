using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookCore.ViewModels
{
    public class CatalogoIndiceViewModel
    {
        public string? Busqueda { get; set; }

        public int? CategoriaId { get; set; }

        public List<SelectListItem> Categorias { get; set; } = [];

        public List<CatalogoLibroViewModel> Libros { get; set; } = [];
    }
}