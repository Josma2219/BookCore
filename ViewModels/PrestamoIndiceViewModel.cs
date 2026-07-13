using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookCore.ViewModels
{
    public class PrestamoIndiceViewModel
    {
        public string? Busqueda { get; set; }

        public string? Estado { get; set; }

        public DateTime? FechaDesde { get; set; }

        public DateTime? FechaHasta { get; set; }

        public List<SelectListItem> Estados { get; set; } = [];

        public List<PrestamoListadoViewModel> Prestamos { get; set; } = [];
    }
}