using BookCore.Data;
using BookCore.Helpers;
using BookCore.Models.Entidades;
using BookCore.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BookCore.Services
{
    public class CatalogoServicio : ICatalogoServicio
    {
        private readonly BookCoreContexto _contexto;

        public CatalogoServicio(
            BookCoreContexto contexto)
        {
            _contexto = contexto;
        }

        public async Task<CatalogoIndiceViewModel>
            ObtenerIndiceAsync(
                string? busqueda,
                int? categoriaId)
        {
            var consulta = _contexto
                .Set<Libro>()
                .AsNoTracking()
                .Include(libro => libro.Categoria)
                .Include(libro => libro.Autor)
                .Include(libro => libro.Ejemplar)
                .AsSplitQuery()
                .Where(libro =>
                    libro.Activo &&
                    libro.Categoria.Activo)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                string textoBusqueda = busqueda.Trim();

                consulta = consulta.Where(libro =>
                    libro.Titulo.Contains(textoBusqueda) ||

                    libro.Autor.Any(autor =>
                        autor.Nombre.Contains(textoBusqueda) ||
                        autor.Apellidos.Contains(textoBusqueda)) ||

                    (
                        libro.Editorial != null &&
                        libro.Editorial.Contains(textoBusqueda)
                    ));
            }

            if (categoriaId.HasValue)
            {
                consulta = consulta.Where(libro =>
                    libro.CategoriaId ==
                    categoriaId.Value);
            }

            var libros = await consulta
                .OrderBy(libro => libro.Titulo)
                .ToListAsync();

            var categorias = await _contexto
                .Set<Categoria>()
                .AsNoTracking()
                .Where(categoria => categoria.Activo)
                .OrderBy(categoria => categoria.Nombre)
                .Select(categoria => new SelectListItem
                {
                    Value = categoria.CategoriaId.ToString(),
                    Text = categoria.Nombre,
                    Selected =
                        categoria.CategoriaId == categoriaId
                })
                .ToListAsync();

            return new CatalogoIndiceViewModel
            {
                Busqueda = busqueda,
                CategoriaId = categoriaId,
                Categorias = categorias,

                Libros = libros.Select(libro =>
                    new CatalogoLibroViewModel
                    {
                        LibroId = libro.LibroId,
                        Titulo = libro.Titulo,

                        Autores = string.Join(
                            ", ",
                            libro.Autor
                                .OrderBy(autor => autor.Apellidos)
                                .ThenBy(autor => autor.Nombre)
                                .Select(autor =>
                                    autor.Nombre + " " +
                                    autor.Apellidos)),

                        CategoriaNombre =
                            libro.Categoria.Nombre,

                        Editorial = libro.Editorial,

                        AnioPublicacion =
                            libro.AnioPublicacion,

                        ImagenUrl = libro.ImagenUrl,

                        Descripcion =
                            libro.Descripcion,

                        TotalEjemplares =
                            libro.Ejemplar.Count(ejemplar =>
                                ejemplar.Activo),

                        EjemplaresDisponibles =
                            libro.Ejemplar.Count(ejemplar =>
                                ejemplar.Activo &&
                                ejemplar.Estado ==
                                EstadosEjemplar.Disponible)
                    })
                    .ToList()
            };
        }

        public async Task<LibroDetalleViewModel?>
            ObtenerDetalleAsync(int libroId)
        {
            var libro = await _contexto
                .Set<Libro>()
                .AsNoTracking()
                .Include(libro => libro.Categoria)
                .Include(libro => libro.Autor)
                .Include(libro => libro.Ejemplar)
                .AsSplitQuery()
                .FirstOrDefaultAsync(libro =>
                    libro.LibroId == libroId &&
                    libro.Activo &&
                    libro.Categoria.Activo);

            if (libro is null)
            {
                return null;
            }

            return new LibroDetalleViewModel
            {
                LibroId = libro.LibroId,
                Titulo = libro.Titulo,
                Isbn = libro.Isbn,
                Editorial = libro.Editorial,
                AnioPublicacion = libro.AnioPublicacion,
                Descripcion = libro.Descripcion,

                CategoriaNombre =
                    libro.Categoria.Nombre,

                Autores = string.Join(
                    ", ",
                    libro.Autor
                        .OrderBy(autor => autor.Apellidos)
                        .ThenBy(autor => autor.Nombre)
                        .Select(autor =>
                            autor.Nombre + " " +
                            autor.Apellidos)),

                ImagenUrl = libro.ImagenUrl,
                Activo = libro.Activo,
                FechaRegistro = libro.FechaRegistro,

                TotalEjemplares =
                    libro.Ejemplar.Count(ejemplar =>
                        ejemplar.Activo),

                EjemplaresDisponibles =
                    libro.Ejemplar.Count(ejemplar =>
                        ejemplar.Activo &&
                        ejemplar.Estado ==
                        EstadosEjemplar.Disponible),

                EjemplaresPrestados =
                    libro.Ejemplar.Count(ejemplar =>
                        ejemplar.Activo &&
                        ejemplar.Estado ==
                        EstadosEjemplar.Prestado),

                EjemplaresDanados =
                    libro.Ejemplar.Count(ejemplar =>
                        ejemplar.Activo &&
                        ejemplar.Estado ==
                        EstadosEjemplar.Danado)
            };
        }
    }
}