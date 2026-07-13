using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace BookCore.ViewModels
{
    public class EjemplarFormularioViewModel
    {
        public int EjemplarId { get; set; }

        [Required(ErrorMessage = "Debes seleccionar un libro.")]
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Debes seleccionar un libro.")]
        [Display(Name = "Libro")]
        public int LibroId { get; set; }

        [Required(ErrorMessage = "El código interno es obligatorio.")]
        [StringLength(
            50,
            ErrorMessage = "El código interno no puede superar los 50 caracteres.")]
        [Display(Name = "Código interno")]
        public string CodigoInterno { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debes seleccionar un estado.")]
        [StringLength(
            20,
            ErrorMessage = "El estado no puede superar los 20 caracteres.")]
        [Display(Name = "Estado")]
        public string Estado { get; set; } = "Disponible";

        [StringLength(
            150,
            ErrorMessage = "La ubicación no puede superar los 150 caracteres.")]
        [Display(Name = "Ubicación")]
        public string? Ubicacion { get; set; }

        [Display(Name = "Ejemplar activo")]
        public bool Activo { get; set; } = true;

        [Display(Name = "Fecha de ingreso")]
        public DateTime FechaIngreso { get; set; }

        public List<SelectListItem> LibrosDisponibles { get; set; } = [];

        public List<SelectListItem> EstadosDisponibles { get; set; } = [];
    }
}