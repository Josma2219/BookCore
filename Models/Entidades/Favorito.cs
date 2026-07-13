using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BookCore.Models.Entidades;

[Index("UsuarioBibliotecaId", "LibroId", Name = "UX_Favorito_Usuario_Libro", IsUnique = true)]
public partial class Favorito
{
    [Key]
    public int FavoritoId { get; set; }

    public int UsuarioBibliotecaId { get; set; }

    public int LibroId { get; set; }

    public DateTime FechaAgregado { get; set; }

    [ForeignKey("LibroId")]
    [InverseProperty("Favorito")]
    public virtual Libro Libro { get; set; } = null!;

    [ForeignKey("UsuarioBibliotecaId")]
    [InverseProperty("Favorito")]
    public virtual UsuarioBiblioteca UsuarioBiblioteca { get; set; } = null!;
}
