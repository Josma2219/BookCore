using BookCore.Data;
using BookCore.Helpers;
using BookCore.Models.Entidades;
using BookCore.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace BookCore.Services
{
    public class FavoritoServicio
        : IFavoritoServicio
    {
        private readonly BookCoreContexto _contexto;

        public FavoritoServicio(
            BookCoreContexto contexto)
        {
            _contexto = contexto;
        }

        public async Task<List<FavoritoLibroViewModel>>
            ObtenerPorUsuarioAsync(
                int usuarioBibliotecaId)
        {
            var favoritos = await _contexto
                .Set<Favorito>()
                .AsNoTracking()
                .Where(favorito =>
                    favorito.UsuarioBibliotecaId ==
                    usuarioBibliotecaId)
                .OrderByDescending(favorito =>
                    favorito.FechaAgregado)
                .ToListAsync();

            var idsLibros = favoritos
                .Select(favorito =>
                    favorito.LibroId)
                .ToList();

            var libros = await _contexto
                .Set<Libro>()
                .AsNoTracking()
                .Where(libro =>
                    idsLibros.Contains(libro.LibroId))
                .Include(libro => libro.Autor)
                .Include(libro => libro.Categoria)
                .Include(libro => libro.Ejemplar)
                .AsSplitQuery()
                .ToListAsync();

            var fechas = favoritos
                .ToDictionary(
                    favorito => favorito.LibroId,
                    favorito => favorito.FechaAgregado);

            var favoritosPorId = favoritos
                .ToDictionary(
                    favorito => favorito.LibroId,
                    favorito => favorito.FavoritoId);

            return libros
                .Select(libro =>
                    new FavoritoLibroViewModel
                    {
                        FavoritoId =
                            favoritosPorId[libro.LibroId],

                        LibroId =
                            libro.LibroId,

                        Titulo =
                            libro.Titulo,

                        Autores = string.Join(
                            ", ",
                            libro.Autor
                                .OrderBy(autor =>
                                    autor.Apellidos)
                                .ThenBy(autor =>
                                    autor.Nombre)
                                .Select(autor =>
                                    autor.Nombre + " " +
                                    autor.Apellidos)),

                        Categoria =
                            libro.Categoria.Nombre,

                        ImagenUrl =
                            libro.ImagenUrl,

                        FechaAgregado =
                            fechas[libro.LibroId],

                        EjemplaresDisponibles =
                            libro.Ejemplar.Count(ejemplar =>
                                ejemplar.Activo &&
                                ejemplar.Estado ==
                                EstadosEjemplar.Disponible)
                    })
                .OrderByDescending(favorito =>
                    favorito.FechaAgregado)
                .ToList();
        }

        public async Task<List<int>> ObtenerIdsAsync(
            int usuarioBibliotecaId)
        {
            return await _contexto
                .Set<Favorito>()
                .AsNoTracking()
                .Where(favorito =>
                    favorito.UsuarioBibliotecaId ==
                    usuarioBibliotecaId)
                .Select(favorito =>
                    favorito.LibroId)
                .ToListAsync();
        }

        public async Task<ResultadoOperacion> AgregarAsync(
            int usuarioBibliotecaId,
            int libroId)
        {
            bool usuarioValido = await _contexto
                .Set<UsuarioBiblioteca>()
                .AnyAsync(usuario =>
                    usuario.UsuarioBibliotecaId ==
                    usuarioBibliotecaId &&
                    usuario.Activo);

            if (!usuarioValido)
            {
                return ResultadoOperacion.Fallido(
                    "El usuario no se encuentra activo.");
            }

            bool libroValido = await _contexto
                .Set<Libro>()
                .AnyAsync(libro =>
                    libro.LibroId == libroId &&
                    libro.Activo);

            if (!libroValido)
            {
                return ResultadoOperacion.Fallido(
                    "El libro no existe o está inactivo.");
            }

            bool yaExiste = await _contexto
                .Set<Favorito>()
                .AnyAsync(favorito =>
                    favorito.UsuarioBibliotecaId ==
                    usuarioBibliotecaId &&
                    favorito.LibroId ==
                    libroId);

            if (yaExiste)
            {
                return ResultadoOperacion.Correcto(
                    "El libro ya estaba en tus favoritos.");
            }

            var favoritoNuevo = new Favorito
            {
                UsuarioBibliotecaId =
                    usuarioBibliotecaId,

                LibroId =
                    libroId,

                FechaAgregado =
                    DateTime.Now
            };

            _contexto
                .Set<Favorito>()
                .Add(favoritoNuevo);

            await _contexto.SaveChangesAsync();

            return ResultadoOperacion.Correcto(
                "El libro se agregó a favoritos.");
        }

        public async Task<ResultadoOperacion> EliminarAsync(
            int usuarioBibliotecaId,
            int libroId)
        {
            var favorito = await _contexto
                .Set<Favorito>()
                .FirstOrDefaultAsync(favoritoActual =>
                    favoritoActual.UsuarioBibliotecaId ==
                    usuarioBibliotecaId &&
                    favoritoActual.LibroId ==
                    libroId);

            if (favorito is null)
            {
                return ResultadoOperacion.Fallido(
                    "El libro no está en tus favoritos.");
            }

            _contexto
                .Set<Favorito>()
                .Remove(favorito);

            await _contexto.SaveChangesAsync();

            return ResultadoOperacion.Correcto(
                "El libro se eliminó de favoritos.");
        }
    }
}