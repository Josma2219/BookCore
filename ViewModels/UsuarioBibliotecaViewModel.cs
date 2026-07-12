using System.ComponentModel.DataAnnotations;

namespace BookCore.ViewModels
{
    public class UsuarioBibliotecaViewModel
    {
        public int UsuarioBibliotecaId { get; set; }

        [Required(ErrorMessage = "El nombre del usuario es obligatorio.")]
        [StringLength(
            100,
            ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "Los apellidos del usuario son obligatorios.")]
        [StringLength(
            150,
            ErrorMessage = "Los apellidos no pueden superar los 150 caracteres.")]
        [Display(Name = "Apellidos")]
        public string Apellidos { get; set; } = string.Empty;

        [Required(ErrorMessage = "La cédula o identificación es obligatoria.")]
        [StringLength(
            50,
            ErrorMessage = "La identificación no puede superar los 50 caracteres.")]
        [Display(Name = "Cédula o identificación")]
        public string Cedula { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "El correo electrónico no tiene un formato válido.")]
        [StringLength(
            150,
            ErrorMessage = "El correo no puede superar los 150 caracteres.")]
        [Display(Name = "Correo electrónico")]
        public string? Correo { get; set; }

        [Phone(ErrorMessage = "El número de teléfono no tiene un formato válido.")]
        [StringLength(
            50,
            ErrorMessage = "El teléfono no puede superar los 50 caracteres.")]
        [Display(Name = "Teléfono")]
        public string? Telefono { get; set; }

        [StringLength(
            300,
            ErrorMessage = "La dirección no puede superar los 300 caracteres.")]
        [Display(Name = "Dirección")]
        public string? Direccion { get; set; }

        [Display(Name = "Usuario activo")]
        public bool Activo { get; set; } = true;

        [Display(Name = "Fecha de registro")]
        public DateTime FechaRegistro { get; set; }

        public string NombreCompleto =>
            $"{Nombre} {Apellidos}".Trim();
    }
}