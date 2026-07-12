using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BookCore.Models.Entidades;

[Index("Correo", Name = "UX_UsuarioAdministrativo_Correo", IsUnique = true)]
[Index("NombreUsuario", Name = "UX_UsuarioAdministrativo_NombreUsuario", IsUnique = true)]
public partial class UsuarioAdministrativo
{
    [Key]
    public int UsuarioAdministrativoId { get; set; }

    [StringLength(80)]
    public string NombreUsuario { get; set; } = null!;

    [StringLength(150)]
    public string Correo { get; set; } = null!;

    [StringLength(500)]
    public string ContrasenaHash { get; set; } = null!;

    [StringLength(50)]
    public string Rol { get; set; } = null!;

    public bool Activo { get; set; }

    public DateTime FechaCreacion { get; set; }
}
