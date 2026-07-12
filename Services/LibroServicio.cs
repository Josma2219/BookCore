using BookCore.Data;
using BookCore.Helpers;
using BookCore.Models.Entidades;
using BookCore.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BookCore.Services
{
    public class LibroServicio : ILibroServicio
    {
        private readonly BookCoreContexto _contexto;
        private readonly ILogger<LibroServicio> _registro;

        public LibroServicio(
            BookCoreContexto contexto,
            ILogger<LibroServicio> registro)
        {
            _contexto = contexto;
            _registro = registro;
        }

        public async Task<LibroIndiceViewModel> ObtenerIndiceAsync(
            string? busqueda,
            int? categoriaId,
            bool soloActivos)
        {
            var consulta = _contexto
                .Set<Libro>()
                .AsNoTracking()
                .Include(libro => libro.Categoria)
                .Include(libro => libro.Autor)
                .Include(libro => libro.Ejemplar)
                .AsSplitQuery()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                string textoBusqueda = busqueda.Trim();

                consulta = consulta.Where(libro =>
                    libro.Titulo.Contains(textoBusqueda) ||
                    (libro.Isbn != null &&
                     libro.Isbn.Contains(textoBusqueda)) ||
                    (libro.Editorial != null &&
                     libro.Editorial.Contains(textoBusqueda)) ||
                    libro.Autor.Any(autor =>
                        autor.Nombre.Contains(textoBusqueda) ||
                        autor.Apellidos.Contains(textoBusqueda)));
            }

            if (categoriaId.HasValue)
            {
                consulta = consulta.Where(libro =>
                    libro.CategoriaId == categoriaId.Value);
            }

            if (soloActivos)
            {
                consulta = consulta.Where(libro => libro.Activo);
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
                    Selected = categoria.CategoriaId == categoriaId
                })
                .ToListAsync();

            return new LibroIndiceViewModel
            {
                Busqueda = busqueda,
                CategoriaId = categoriaId,
                SoloActivos = soloActivos,
                Categorias = categorias,

                Libros = libros.Select(libro =>
                    new LibroListadoViewModel
                    {
                        LibroId = libro.LibroId,
                        Titulo = libro.Titulo,
                        Isbn = libro.Isbn,
                        Editorial = libro.Editorial,
                        AnioPublicacion = libro.AnioPublicacion,
                        CategoriaNombre = libro.Categoria.Nombre,

                        Autores = string.Join(
                            ", ",
                            libro.Autor
                                .OrderBy(autor => autor.Apellidos)
                                .ThenBy(autor => autor.Nombre)
                                .Select(autor =>
                                    $"{autor.Nombre} {autor.Apellidos}")),

                        Activo = libro.Activo,

                        TotalEjemplares = libro.Ejemplar.Count(
                            ejemplar => ejemplar.Activo),

                        EjemplaresDisponibles = libro.Ejemplar.Count(
                            ejemplar =>
                                ejemplar.Activo &&
                                ejemplar.Estado == "Disponible")
                    })
                    .ToList()
            };
        }

        public async Task<LibroDetalleViewModel?> ObtenerDetalleAsync(
            int libroId)
        {
            var libro = await _contexto
                .Set<Libro>()
                .AsNoTracking()
                .Include(libro => libro.Categoria)
                .Include(libro => libro.Autor)
                .Include(libro => libro.Ejemplar)
                .AsSplitQuery()
                .FirstOrDefaultAsync(libro =>
                    libro.LibroId == libroId);

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
                CategoriaNombre = libro.Categoria.Nombre,

                Autores = string.Join(
                    ", ",
                    libro.Autor
                        .OrderBy(autor => autor.Apellidos)
                        .ThenBy(autor => autor.Nombre)
                        .Select(autor =>
                            $"{autor.Nombre} {autor.Apellidos}")),

                ImagenUrl = libro.ImagenUrl,
                Activo = libro.Activo,
                FechaRegistro = libro.FechaRegistro,

                TotalEjemplares = libro.Ejemplar.Count(
                    ejemplar => ejemplar.Activo),

                EjemplaresDisponibles = libro.Ejemplar.Count(
                    ejemplar =>
                        ejemplar.Activo &&
                        ejemplar.Estado == "Disponible"),

                EjemplaresPrestados = libro.Ejemplar.Count(
                    ejemplar =>
                        ejemplar.Activo &&
                        ejemplar.Estado == "Prestado"),

                EjemplaresDanados = libro.Ejemplar.Count(
                    ejemplar =>
                        ejemplar.Activo &&
                        ejemplar.Estado == "Dañado")
            };
        }

        public async Task<LibroFormularioViewModel> PrepararCreacionAsync()
        {
            var modelo = new LibroFormularioViewModel();

            return await CargarOpcionesAsync(modelo);
        }

        public async Task<LibroFormularioViewModel?> PrepararEdicionAsync(
            int libroId)
        {
            var libro = await _contexto
                .Set<Libro>()
                .AsNoTracking()
                .Include(libro => libro.Autor)
                .FirstOrDefaultAsync(libro =>
                    libro.LibroId == libroId);

            if (libro is null)
            {
                return null;
            }

            var modelo = new LibroFormularioViewModel
            {
                LibroId = libro.LibroId,
                Titulo = libro.Titulo,
                Isbn = libro.Isbn,
                Editorial = libro.Editorial,
                AnioPublicacion = libro.AnioPublicacion,
                Descripcion = libro.Descripcion,
                CategoriaId = libro.CategoriaId,
                ImagenUrl = libro.ImagenUrl,
                Activo = libro.Activo,
                FechaRegistro = libro.FechaRegistro,

                AutoresSeleccionados = libro.Autor
                    .Select(autor => autor.AutorId)
                    .ToList()
            };

            return await CargarOpcionesAsync(modelo);
        }

        public async Task<LibroFormularioViewModel> CargarOpcionesAsync(
            LibroFormularioViewModel modelo)
        {
            var autoresSeleccionados = modelo.AutoresSeleccionados
                .Distinct()
                .ToList();

            modelo.CategoriasDisponibles = await _contexto
                .Set<Categoria>()
                .AsNoTracking()
                .Where(categoria =>
                    categoria.Activo ||
                    categoria.CategoriaId == modelo.CategoriaId)
                .OrderBy(categoria => categoria.Nombre)
                .Select(categoria => new SelectListItem
                {
                    Value = categoria.CategoriaId.ToString(),
                    Text = categoria.Nombre,
                    Selected =
                        categoria.CategoriaId == modelo.CategoriaId
                })
                .ToListAsync();

            modelo.AutoresDisponibles = await _contexto
                .Set<Autor>()
                .AsNoTracking()
                .Where(autor =>
                    autor.Activo ||
                    autoresSeleccionados.Contains(autor.AutorId))
                .OrderBy(autor => autor.Apellidos)
                .ThenBy(autor => autor.Nombre)
                .Select(autor => new SelectListItem
                {
                    Value = autor.AutorId.ToString(),
                    Text = autor.Nombre + " " + autor.Apellidos,
                    Selected =
                        autoresSeleccionados.Contains(autor.AutorId)
                })
                .ToListAsync();

            return modelo;
        }

        public async Task<ResultadoOperacion> CrearAsync(
            LibroFormularioViewModel modelo)
        {
            string tituloLimpio = modelo.Titulo.Trim();
            string? isbnLimpio = LimpiarTextoOpcional(modelo.Isbn);

            var validacion = await ValidarDatosAsync(
                modelo,
                isbnLimpio,
                null);

            if (validacion is not null)
            {
                return validacion;
            }

            var autores = await ObtenerAutoresSeleccionadosAsync(
                modelo.AutoresSeleccionados);

            if (autores is null)
            {
                return ResultadoOperacion.Fallido(
                    "Uno o varios autores seleccionados no existen o están inactivos.");
            }

            var libroNuevo = new Libro
            {
                Titulo = tituloLimpio,
                Isbn = isbnLimpio,
                Editorial = LimpiarTextoOpcional(modelo.Editorial),
                AnioPublicacion = modelo.AnioPublicacion,
                Descripcion = LimpiarTextoOpcional(modelo.Descripcion),
                CategoriaId = modelo.CategoriaId,
                ImagenUrl = LimpiarTextoOpcional(modelo.ImagenUrl),
                Activo = true,
                FechaRegistro = DateTime.Now
            };

            foreach (var autor in autores)
            {
                libroNuevo.Autor.Add(autor);
            }

            try
            {
                _contexto.Set<Libro>().Add(libroNuevo);
                await _contexto.SaveChangesAsync();

                return ResultadoOperacion.Correcto(
                    "El libro se creó correctamente.");
            }
            catch (DbUpdateException excepcion)
            {
                _registro.LogError(
                    excepcion,
                    "Error al crear el libro {Titulo}.",
                    tituloLimpio);

                return ResultadoOperacion.Fallido(
                    "No fue posible guardar el libro.");
            }
        }

        public async Task<ResultadoOperacion> EditarAsync(
            LibroFormularioViewModel modelo)
        {
            var libro = await _contexto
                .Set<Libro>()
                .Include(libro => libro.Autor)
                .FirstOrDefaultAsync(libro =>
                    libro.LibroId == modelo.LibroId);

            if (libro is null)
            {
                return ResultadoOperacion.Fallido(
                    "El libro que intentas editar no existe.");
            }

            string? isbnLimpio = LimpiarTextoOpcional(modelo.Isbn);

            var validacion = await ValidarDatosAsync(
                modelo,
                isbnLimpio,
                modelo.LibroId);

            if (validacion is not null)
            {
                return validacion;
            }

            if (libro.Activo && !modelo.Activo)
            {
                bool tienePrestamosActivos =
                    await TienePrestamosActivosAsync(modelo.LibroId);

                if (tienePrestamosActivos)
                {
                    return ResultadoOperacion.Fallido(
                        "No puedes desactivar este libro porque tiene préstamos activos.");
                }
            }

            var autores = await ObtenerAutoresSeleccionadosAsync(
                modelo.AutoresSeleccionados);

            if (autores is null)
            {
                return ResultadoOperacion.Fallido(
                    "Uno o varios autores seleccionados no existen o están inactivos.");
            }

            libro.Titulo = modelo.Titulo.Trim();
            libro.Isbn = isbnLimpio;
            libro.Editorial = LimpiarTextoOpcional(modelo.Editorial);
            libro.AnioPublicacion = modelo.AnioPublicacion;
            libro.Descripcion = LimpiarTextoOpcional(modelo.Descripcion);
            libro.CategoriaId = modelo.CategoriaId;
            libro.ImagenUrl = LimpiarTextoOpcional(modelo.ImagenUrl);
            libro.Activo = modelo.Activo;

            libro.Autor.Clear();

            foreach (var autor in autores)
            {
                libro.Autor.Add(autor);
            }

            try
            {
                await _contexto.SaveChangesAsync();

                return ResultadoOperacion.Correcto(
                    "El libro se actualizó correctamente.");
            }
            catch (DbUpdateException excepcion)
            {
                _registro.LogError(
                    excepcion,
                    "Error al editar el libro {LibroId}.",
                    modelo.LibroId);

                return ResultadoOperacion.Fallido(
                    "No fue posible actualizar el libro.");
            }
        }

        public async Task<ResultadoOperacion> EliminarAsync(
            int libroId)
        {
            var libro = await _contexto
                .Set<Libro>()
                .FirstOrDefaultAsync(libro =>
                    libro.LibroId == libroId);

            if (libro is null)
            {
                return ResultadoOperacion.Fallido(
                    "El libro que intentas eliminar no existe.");
            }

            if (!libro.Activo)
            {
                return ResultadoOperacion.Fallido(
                    "El libro ya se encuentra inactivo.");
            }

            bool tienePrestamosActivos =
                await TienePrestamosActivosAsync(libroId);

            if (tienePrestamosActivos)
            {
                return ResultadoOperacion.Fallido(
                    "No puedes eliminar este libro porque tiene préstamos activos.");
            }

            // Se conserva el libro para no perder relaciones o historial.
            libro.Activo = false;

            try
            {
                await _contexto.SaveChangesAsync();

                return ResultadoOperacion.Correcto(
                    "El libro se desactivó correctamente.");
            }
            catch (DbUpdateException excepcion)
            {
                _registro.LogError(
                    excepcion,
                    "Error al desactivar el libro {LibroId}.",
                    libroId);

                return ResultadoOperacion.Fallido(
                    "No fue posible desactivar el libro.");
            }
        }

        private async Task<ResultadoOperacion?> ValidarDatosAsync(
            LibroFormularioViewModel modelo,
            string? isbnLimpio,
            int? libroIdActual)
        {
            if (modelo.AnioPublicacion.HasValue &&
                modelo.AnioPublicacion.Value > DateTime.Today.Year)
            {
                return ResultadoOperacion.Fallido(
                    "El año de publicación no puede ser posterior al año actual.");
            }

            bool categoriaValida = await _contexto
                .Set<Categoria>()
                .AnyAsync(categoria =>
                    categoria.CategoriaId == modelo.CategoriaId &&
                    categoria.Activo);

            if (!categoriaValida)
            {
                return ResultadoOperacion.Fallido(
                    "La categoría seleccionada no existe o está inactiva.");
            }

            if (modelo.AutoresSeleccionados.Count == 0)
            {
                return ResultadoOperacion.Fallido(
                    "Debes seleccionar al menos un autor.");
            }

            if (!string.IsNullOrWhiteSpace(isbnLimpio))
            {
                bool isbnRepetido = await _contexto
                    .Set<Libro>()
                    .AnyAsync(libro =>
                        libro.Isbn == isbnLimpio &&
                        (!libroIdActual.HasValue ||
                         libro.LibroId != libroIdActual.Value));

                if (isbnRepetido)
                {
                    return ResultadoOperacion.Fallido(
                        "Ya existe otro libro con ese ISBN.");
                }
            }

            return null;
        }

        private async Task<List<Autor>?> ObtenerAutoresSeleccionadosAsync(
            List<int> idsAutores)
        {
            var idsSinRepetir = idsAutores
                .Distinct()
                .ToList();

            var autores = await _contexto
                .Set<Autor>()
                .Where(autor =>
                    idsSinRepetir.Contains(autor.AutorId) &&
                    autor.Activo)
                .ToListAsync();

            if (autores.Count != idsSinRepetir.Count)
            {
                return null;
            }

            return autores;
        }

        private async Task<bool> TienePrestamosActivosAsync(
            int libroId)
        {
            return await (
                from prestamo in _contexto.Set<Prestamo>()
                join ejemplar in _contexto.Set<Ejemplar>()
                    on prestamo.EjemplarId equals ejemplar.EjemplarId
                where ejemplar.LibroId == libroId &&
                      prestamo.Estado == "Activo"
                select prestamo
            ).AnyAsync();
        }

        private static string? LimpiarTextoOpcional(string? texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
            {
                return null;
            }

            return texto.Trim();
        }
    }
}