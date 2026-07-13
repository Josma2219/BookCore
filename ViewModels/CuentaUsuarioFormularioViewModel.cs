using System.ComponentModel.DataAnnotations;

namespace BookCore.ViewModels
{
    public class CuentaUsuarioFormularioViewModel
    {
        public int CuentaUsuarioId { get; set; }

        public int UsuarioBibliotecaId { get; set; }

        public string UsuarioNombreCompleto { get; set; }
            = string.Empty;

        public bool TieneCuenta { get; set; }

        [Required(
            ErrorMessage = "El nombre de usuario es obligatorio.")]
        [StringLength(
            100,
            MinimumLength = 4,
            ErrorMessage =
                "El nombre de usuario debe tener entre 4 y 100 caracteres.")]
        [Display(Name = "Nombre de usuario")]
        public string NombreUsuario { get; set; }
            = string.Empty;

        [Required(
            ErrorMessage = "El correo de acceso es obligatorio.")]
        [EmailAddress(
            ErrorMessage = "El correo no tiene un formato válido.")]
        [StringLength(
            150,
            ErrorMessage =
                "El correo no puede superar los 150 caracteres.")]
        [Display(Name = "Correo de acceso")]
        public string Correo { get; set; }
            = string.Empty;

        [DataType(DataType.Password)]
        [StringLength(
            100,
            MinimumLength = 8,
            ErrorMessage =
                "La contraseña debe tener al menos 8 caracteres.")]
        [Display(Name = "Nueva contraseña")]
        public string? ContrasenaNueva { get; set; }

        [DataType(DataType.Password)]
        [Compare(
            nameof(ContrasenaNueva),
            ErrorMessage = "Las contraseñas no coinciden.")]
        [Display(Name = "Confirmar contraseña")]
        public string? ConfirmarContrasena { get; set; }

        [Display(Name = "Cuenta activa")]
        public bool Activo { get; set; } = true;
    }
}