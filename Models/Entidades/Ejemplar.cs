using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BookCore.Models.Entidades;

[Index("CodigoInterno", Name = "UX_Ejemplar_CodigoInterno", IsUnique = true)]
public partial class Ejemplar
{
    [Key]
    public int EjemplarId { get; set; }

    public int LibroId { get; set; }

    [StringLength(50)]
    public string CodigoInterno { get; set; } = null!;

    [StringLength(20)]
    public string Estado { get; set; } = null!;

    [StringLength(150)]
    public string? Ubicacion { get; set; }

    public DateTime FechaIngreso { get; set; }

    public bool Activo { get; set; }

    [ForeignKey("LibroId")]
    [InverseProperty("Ejemplar")]
    public virtual Libro Libro { get; set; } = null!;

    [InverseProperty("Ejemplar")]
    public virtual ICollection<Prestamo> Prestamo { get; set; } = new List<Prestamo>();
}
