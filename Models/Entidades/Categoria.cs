using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BookCore.Models.Entidades;

[Index("Nombre", Name = "UX_Categoria_Nombre", IsUnique = true)]
public partial class Categoria
{
    [Key]
    public int CategoriaId { get; set; }

    [StringLength(100)]
    public string Nombre { get; set; } = null!;

    [StringLength(300)]
    public string? Descripcion { get; set; }

    public bool Activo { get; set; }

    public DateTime FechaCreacion { get; set; }

    [InverseProperty("Categoria")]
    public virtual ICollection<Libro> Libro { get; set; } = new List<Libro>();
}
