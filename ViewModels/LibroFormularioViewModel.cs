using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace BookCore.ViewModels
{
    public class LibroFormularioViewModel
    {
        public int LibroId { get; set; }

        [Required(ErrorMessage = "El título del libro es obligatorio.")]
        [StringLength(
            200,
            ErrorMessage = "El título no puede superar los 200 caracteres.")]
        [Display(Name = "Título")]
        public string Titulo { get; set; } = string.Empty;

        [StringLength(
            30,
            ErrorMessage = "El ISBN no puede superar los 30 caracteres.")]
        [Display(Name = "ISBN")]
        public string? Isbn { get; set; }

        [StringLength(
            150,
            ErrorMessage = "La editorial no puede superar los 150 caracteres.")]
        [Display(Name = "Editorial")]
        public string? Editorial { get; set; }

        [Range(
            1,
            9999,
            ErrorMessage = "El año de publicación no es válido.")]
        [Display(Name = "Año de publicación")]
        public int? AnioPublicacion { get; set; }

        [StringLength(
            1500,
            ErrorMessage = "La descripción no puede superar los 1500 caracteres.")]
        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }

        [Required(ErrorMessage = "Debes seleccionar una categoría.")]
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Debes seleccionar una categoría.")]
        [Display(Name = "Categoría")]
        public int CategoriaId { get; set; }

        [StringLength(
            500,
            ErrorMessage = "La dirección de la imagen no puede superar los 500 caracteres.")]
        [Display(Name = "Dirección de la portada")]
        public string? ImagenUrl { get; set; }

        [Display(Name = "Libro activo")]
        public bool Activo { get; set; } = true;

        [Display(Name = "Fecha de registro")]
        public DateTime FechaRegistro { get; set; }

        [MinLength(
            1,
            ErrorMessage = "Debes seleccionar al menos un autor.")]
        [Display(Name = "Autores")]
        public List<int> AutoresSeleccionados { get; set; } = [];

        public List<SelectListItem> CategoriasDisponibles { get; set; } = [];

        public List<SelectListItem> AutoresDisponibles { get; set; } = [];
    }
}