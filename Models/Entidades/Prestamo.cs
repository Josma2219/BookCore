using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BookCore.Models.Entidades;

public partial class Prestamo
{
    [Key]
    public int PrestamoId { get; set; }

    public int UsuarioBibliotecaId { get; set; }

    public int EjemplarId { get; set; }

    public DateTime FechaPrestamo { get; set; }

    public DateTime FechaVencimiento { get; set; }

    public DateTime? FechaDevolucion { get; set; }

    [StringLength(20)]
    public string Estado { get; set; } = null!;

    [StringLength(500)]
    public string? Observaciones { get; set; }

    [ForeignKey("EjemplarId")]
    [InverseProperty("Prestamo")]
    public virtual Ejemplar Ejemplar { get; set; } = null!;

    [ForeignKey("UsuarioBibliotecaId")]
    [InverseProperty("Prestamo")]
    public virtual UsuarioBiblioteca UsuarioBiblioteca { get; set; } = null!;
}
