using System;
using System.Collections.Generic;
using BookCore.Models.Entidades;
using Microsoft.EntityFrameworkCore;

namespace BookCore.Data;

public partial class BookCoreContexto : DbContext
{
    public BookCoreContexto(DbContextOptions<BookCoreContexto> options)
        : base(options)
    {
    }

    public virtual DbSet<Autor> Autor { get; set; }

    public virtual DbSet<Categoria> Categoria { get; set; }

    public virtual DbSet<CuentaUsuario> CuentaUsuario { get; set; }

    public virtual DbSet<Ejemplar> Ejemplar { get; set; }

    public virtual DbSet<Favorito> Favorito { get; set; }

    public virtual DbSet<Libro> Libro { get; set; }

    public virtual DbSet<Prestamo> Prestamo { get; set; }

    public virtual DbSet<UsuarioAdministrativo> UsuarioAdministrativo { get; set; }

    public virtual DbSet<UsuarioBiblioteca> UsuarioBiblioteca { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Autor>(entity =>
        {
            entity.HasKey(e => e.AutorId).HasName("PK__Autor__F58AE929C4974415");

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.HasKey(e => e.CategoriaId).HasName("PK__Categori__F353C1E5DBA3235D");

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<CuentaUsuario>(entity =>
        {
            entity.Property(e => e.Activo).HasDefaultValue(true, "DF_CuentaUsuario_Activo");
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())", "DF_CuentaUsuario_FechaCreacion");

            entity.HasOne(d => d.UsuarioBiblioteca).WithOne(p => p.CuentaUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CuentaUsuario_UsuarioBiblioteca");
        });

        modelBuilder.Entity<Ejemplar>(entity =>
        {
            entity.HasKey(e => e.EjemplarId).HasName("PK__Ejemplar__C7803E497D318E13");

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Estado).HasDefaultValue("Disponible");
            entity.Property(e => e.FechaIngreso).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Libro).WithMany(p => p.Ejemplar)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ejemplar_Libro");
        });

        modelBuilder.Entity<Favorito>(entity =>
        {
            entity.Property(e => e.FechaAgregado).HasDefaultValueSql("(getdate())", "DF_Favorito_FechaAgregado");

            entity.HasOne(d => d.Libro).WithMany(p => p.Favorito)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Favorito_Libro");

            entity.HasOne(d => d.UsuarioBiblioteca).WithMany(p => p.Favorito)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Favorito_UsuarioBiblioteca");
        });

        modelBuilder.Entity<Libro>(entity =>
        {
            entity.HasKey(e => e.LibroId).HasName("PK__Libro__35A1ECED7DFFCC09");

            entity.HasIndex(e => e.Isbn, "UX_Libro_Isbn")
                .IsUnique()
                .HasFilter("([Isbn] IS NOT NULL)");

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.FechaRegistro).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Categoria).WithMany(p => p.Libro)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Libro_Categoria");

            entity.HasMany(d => d.Autor).WithMany(p => p.Libro)
                .UsingEntity<Dictionary<string, object>>(
                    "LibroAutor",
                    r => r.HasOne<Autor>().WithMany()
                        .HasForeignKey("AutorId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_LibroAutor_Autor"),
                    l => l.HasOne<Libro>().WithMany()
                        .HasForeignKey("LibroId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_LibroAutor_Libro"),
                    j =>
                    {
                        j.HasKey("LibroId", "AutorId");
                    });
        });

        modelBuilder.Entity<Prestamo>(entity =>
        {
            entity.HasKey(e => e.PrestamoId).HasName("PK__Prestamo__AA58A0A0CB64474B");

            entity.Property(e => e.Estado).HasDefaultValue("Activo");
            entity.Property(e => e.FechaPrestamo).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Ejemplar).WithMany(p => p.Prestamo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Prestamo_Ejemplar");

            entity.HasOne(d => d.UsuarioBiblioteca).WithMany(p => p.Prestamo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Prestamo_UsuarioBiblioteca");
        });

        modelBuilder.Entity<UsuarioAdministrativo>(entity =>
        {
            entity.HasKey(e => e.UsuarioAdministrativoId).HasName("PK__UsuarioA__A0F45DB7A9E6BC1A");

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Rol).HasDefaultValue("Administrador");
        });

        modelBuilder.Entity<UsuarioBiblioteca>(entity =>
        {
            entity.HasKey(e => e.UsuarioBibliotecaId).HasName("PK__UsuarioB__CEA3C62664B2B41F");

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.FechaRegistro).HasDefaultValueSql("(getdate())");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
