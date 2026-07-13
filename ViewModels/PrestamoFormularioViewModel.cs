using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace BookCore.ViewModels
{
    public class PrestamoFormularioViewModel
    {
        [Required(ErrorMessage = "Debes seleccionar un usuario.")]
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Debes seleccionar un usuario.")]
        [Display(Name = "Usuario de biblioteca")]
        public int UsuarioBibliotecaId { get; set; }

        [Required(ErrorMessage = "Debes seleccionar un libro.")]
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Debes seleccionar un libro.")]
        [Display(Name = "Libro")]
        public int LibroId { get; set; }

        [Required(ErrorMessage = "Debes seleccionar un ejemplar.")]
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Debes seleccionar un ejemplar.")]
        [Display(Name = "Ejemplar disponible")]
        public int EjemplarId { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha de vencimiento")]
        public DateTime FechaVencimiento { get; set; }

        [StringLength(
            500,
            ErrorMessage = "Las observaciones no pueden superar los 500 caracteres.")]
        [Display(Name = "Observaciones")]
        public string? Observaciones { get; set; }

        public List<SelectListItem> UsuariosDisponibles { get; set; } = [];

        public List<SelectListItem> LibrosDisponibles { get; set; } = [];

        public List<SelectListItem> EjemplaresDisponibles { get; set; } = [];
    }
}