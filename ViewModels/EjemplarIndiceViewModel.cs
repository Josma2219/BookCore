using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookCore.ViewModels
{
    public class EjemplarIndiceViewModel
    {
        public string? Busqueda { get; set; }

        public int? LibroId { get; set; }

        public string? Estado { get; set; }

        public bool SoloActivos { get; set; }

        public List<SelectListItem> Libros { get; set; } = [];

        public List<SelectListItem> Estados { get; set; } = [];

        public List<EjemplarListadoViewModel> Ejemplares { get; set; } = [];
    }
}