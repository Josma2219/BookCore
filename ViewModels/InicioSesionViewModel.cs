using System.ComponentModel.DataAnnotations;

namespace BookCore.ViewModels
{
    public class InicioSesionViewModel
    {
        [Required(
            ErrorMessage = "Debes escribir tu usuario o correo.")]
        [Display(Name = "Usuario o correo")]
        public string Identificador { get; set; } = string.Empty;

        [Required(
            ErrorMessage = "Debes escribir tu contraseña.")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Contrasena { get; set; } = string.Empty;

        [Display(Name = "Mantener sesión iniciada")]
        public bool Recordarme { get; set; }

        public string? ReturnUrl { get; set; }
    }
}