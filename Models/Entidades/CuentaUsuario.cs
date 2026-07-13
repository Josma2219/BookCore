using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BookCore.Models.Entidades;

[Index("Correo", Name = "UX_CuentaUsuario_Correo", IsUnique = true)]
[Index("NombreUsuario", Name = "UX_CuentaUsuario_NombreUsuario", IsUnique = true)]
[Index("UsuarioBibliotecaId", Name = "UX_CuentaUsuario_UsuarioBibliotecaId", IsUnique = true)]
public partial class CuentaUsuario
{
    [Key]
    public int CuentaUsuarioId { get; set; }

    public int UsuarioBibliotecaId { get; set; }

    [StringLength(100)]
    public string NombreUsuario { get; set; } = null!;

    [StringLength(150)]
    public string Correo { get; set; } = null!;

    [StringLength(500)]
    public string ContrasenaHash { get; set; } = null!;

    public bool Activo { get; set; }

    public DateTime FechaCreacion { get; set; }

    [ForeignKey("UsuarioBibliotecaId")]
    [InverseProperty("CuentaUsuario")]
    public virtual UsuarioBiblioteca UsuarioBiblioteca { get; set; } = null!;
}
