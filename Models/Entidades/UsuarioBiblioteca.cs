using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BookCore.Models.Entidades;

[Index("Cedula", Name = "UX_UsuarioBiblioteca_Cedula", IsUnique = true)]
public partial class UsuarioBiblioteca
{
    [Key]
    public int UsuarioBibliotecaId { get; set; }

    [StringLength(100)]
    public string Nombre { get; set; } = null!;

    [StringLength(150)]
    public string Apellidos { get; set; } = null!;

    [StringLength(50)]
    public string Cedula { get; set; } = null!;

    [StringLength(150)]
    public string? Correo { get; set; }

    [StringLength(50)]
    public string? Telefono { get; set; }

    [StringLength(300)]
    public string? Direccion { get; set; }

    public bool Activo { get; set; }

    public DateTime FechaRegistro { get; set; }

    [InverseProperty("UsuarioBiblioteca")]
    public virtual ICollection<Prestamo> Prestamo { get; set; } = new List<Prestamo>();
}
