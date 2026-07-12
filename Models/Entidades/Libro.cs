using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BookCore.Models.Entidades;

public partial class Libro
{
    [Key]
    public int LibroId { get; set; }

    [StringLength(200)]
    public string Titulo { get; set; } = null!;

    [StringLength(30)]
    public string? Isbn { get; set; }

    [StringLength(150)]
    public string? Editorial { get; set; }

    public int? AnioPublicacion { get; set; }

    [StringLength(1500)]
    public string? Descripcion { get; set; }

    public int CategoriaId { get; set; }

    [StringLength(500)]
    public string? ImagenUrl { get; set; }

    public bool Activo { get; set; }

    public DateTime FechaRegistro { get; set; }

    [ForeignKey("CategoriaId")]
    [InverseProperty("Libro")]
    public virtual Categoria Categoria { get; set; } = null!;

    [InverseProperty("Libro")]
    public virtual ICollection<Ejemplar> Ejemplar { get; set; } = new List<Ejemplar>();

    [ForeignKey("LibroId")]
    [InverseProperty("Libro")]
    public virtual ICollection<Autor> Autor { get; set; } = new List<Autor>();
}
