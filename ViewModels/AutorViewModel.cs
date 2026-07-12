using System.ComponentModel.DataAnnotations;

namespace BookCore.ViewModels
{
    public class AutorViewModel
    {
        public int AutorId { get; set; }

        [Required(ErrorMessage = "El nombre del autor es obligatorio.")]
        [StringLength(
            100,
            ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "Los apellidos del autor son obligatorios.")]
        [StringLength(
            150,
            ErrorMessage = "Los apellidos no pueden superar los 150 caracteres.")]
        [Display(Name = "Apellidos")]
        public string Apellidos { get; set; } = string.Empty;

        [StringLength(
            100,
            ErrorMessage = "La nacionalidad no puede superar los 100 caracteres.")]
        [Display(Name = "Nacionalidad")]
        public string? Nacionalidad { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha de nacimiento")]
        public DateOnly? FechaNacimiento { get; set; }

        [StringLength(
            1000,
            ErrorMessage = "La biografía no puede superar los 1000 caracteres.")]
        [Display(Name = "Biografía")]
        public string? Biografia { get; set; }

        [Display(Name = "Autor activo")]
        public bool Activo { get; set; } = true;

        [Display(Name = "Fecha de creación")]
        public DateTime FechaCreacion { get; set; }

        public string NombreCompleto
        {
            get
            {
                return $"{Nombre} {Apellidos}".Trim();
            }
        }
    }
}