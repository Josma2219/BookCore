using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BookCore.Models.Entidades;

public partial class Autor
{
    [Key]
    public int AutorId { get; set; }

    [StringLength(100)]
    public string Nombre { get; set; } = null!;

    [StringLength(150)]
    public string Apellidos { get; set; } = null!;

    [StringLength(100)]
    public string? Nacionalidad { get; set; }

    public DateOnly? FechaNacimiento { get; set; }

    [StringLength(1000)]
    public string? Biografia { get; set; }

    public bool Activo { get; set; }

    public DateTime FechaCreacion { get; set; }

    [ForeignKey("AutorId")]
    [InverseProperty("Autor")]
    public virtual ICollection<Libro> Libro { get; set; } = new List<Libro>();
}
