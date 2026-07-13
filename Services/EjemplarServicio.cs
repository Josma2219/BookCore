using BookCore.Data;
using BookCore.Helpers;
using BookCore.Models.Entidades;
using BookCore.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BookCore.Services
{
    public class EjemplarServicio : IEjemplarServicio
    {
        private readonly BookCoreContexto _contexto;
        private readonly ILogger<EjemplarServicio> _registro;

        public EjemplarServicio(
            BookCoreContexto contexto,
            ILogger<EjemplarServicio> registro)
        {
            _contexto = contexto;
            _registro = registro;
        }

        public async Task<EjemplarIndiceViewModel> ObtenerIndiceAsync(
            string? busqueda,
            int? libroId,
            string? estado,
            bool soloActivos)
        {
            var consulta = _contexto
                .Set<Ejemplar>()
                .AsNoTracking()
                .Include(ejemplar => ejemplar.Libro)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                string textoBusqueda = busqueda.Trim();

                consulta = consulta.Where(ejemplar =>
                    ejemplar.CodigoInterno.Contains(textoBusqueda) ||
                    ejemplar.Libro.Titulo.Contains(textoBusqueda) ||
                    (ejemplar.Ubicacion != null &&
                     ejemplar.Ubicacion.Contains(textoBusqueda)));
            }

            if (libroId.HasValue)
            {
                consulta = consulta.Where(ejemplar =>
                    ejemplar.LibroId == libroId.Value);
            }

            if (!string.IsNullOrWhiteSpace(estado))
            {
                consulta = consulta.Where(ejemplar =>
                    ejemplar.Estado == estado);
            }

            if (soloActivos)
            {
                consulta = consulta.Where(ejemplar =>
                    ejemplar.Activo);
            }

            var ejemplares = await consulta
                .OrderBy(ejemplar => ejemplar.Libro.Titulo)
                .ThenBy(ejemplar => ejemplar.CodigoInterno)
                .Select(ejemplar => new EjemplarListadoViewModel
                {
                    EjemplarId = ejemplar.EjemplarId,
                    LibroId = ejemplar.LibroId,
                    LibroTitulo = ejemplar.Libro.Titulo,
                    CodigoInterno = ejemplar.CodigoInterno,
                    Estado = ejemplar.Estado,
                    Ubicacion = ejemplar.Ubicacion,
                    FechaIngreso = ejemplar.FechaIngreso,
                    Activo = ejemplar.Activo,

                    TienePrestamoActivo = _contexto
                        .Set<Prestamo>()
                        .Any(prestamo =>
                            prestamo.EjemplarId ==
                            ejemplar.EjemplarId &&
                            (
    prestamo.Estado == EstadosPrestamo.Activo ||
    prestamo.Estado == EstadosPrestamo.Vencido
))
                })
                .ToListAsync();

            return new EjemplarIndiceViewModel
            {
                Busqueda = busqueda,
                LibroId = libroId,
                Estado = estado,
                SoloActivos = soloActivos,
                Ejemplares = ejemplares,
                Libros = await ObtenerOpcionesLibrosAsync(libroId),
                Estados = CrearOpcionesEstados(estado, true)
            };
        }

        public async Task<EjemplarListadoViewModel?> ObtenerPorIdAsync(
            int ejemplarId)
        {
            return await _contexto
                .Set<Ejemplar>()
                .AsNoTracking()
                .Where(ejemplar =>
                    ejemplar.EjemplarId == ejemplarId)
                .Select(ejemplar => new EjemplarListadoViewModel
                {
                    EjemplarId = ejemplar.EjemplarId,
                    LibroId = ejemplar.LibroId,
                    LibroTitulo = ejemplar.Libro.Titulo,
                    CodigoInterno = ejemplar.CodigoInterno,
                    Estado = ejemplar.Estado,
                    Ubicacion = ejemplar.Ubicacion,
                    FechaIngreso = ejemplar.FechaIngreso,
                    Activo = ejemplar.Activo,

                    TienePrestamoActivo = _contexto
                        .Set<Prestamo>()
                        .Any(prestamo =>
                            prestamo.EjemplarId ==
                            ejemplar.EjemplarId &&
                            (
    prestamo.Estado == EstadosPrestamo.Activo ||
    prestamo.Estado == EstadosPrestamo.Vencido
))
                })
                .FirstOrDefaultAsync();
        }

        public async Task<EjemplarFormularioViewModel>
            PrepararCreacionAsync(int? libroId = null)
        {
            var modelo = new EjemplarFormularioViewModel
            {
                LibroId = libroId ?? 0,
                Estado = EstadosEjemplar.Disponible,
                Activo = true
            };

            return await CargarOpcionesAsync(modelo);
        }

        public async Task<EjemplarFormularioViewModel?>
            PrepararEdicionAsync(int ejemplarId)
        {
            var ejemplar = await _contexto
                .Set<Ejemplar>()
                .AsNoTracking()
                .FirstOrDefaultAsync(ejemplar =>
                    ejemplar.EjemplarId == ejemplarId);

            if (ejemplar is null)
            {
                return null;
            }

            var modelo = new EjemplarFormularioViewModel
            {
                EjemplarId = ejemplar.EjemplarId,
                LibroId = ejemplar.LibroId,
                CodigoInterno = ejemplar.CodigoInterno,
                Estado = ejemplar.Estado,
                Ubicacion = ejemplar.Ubicacion,
                Activo = ejemplar.Activo,
                FechaIngreso = ejemplar.FechaIngreso
            };

            return await CargarOpcionesAsync(modelo);
        }

        public async Task<EjemplarFormularioViewModel>
            CargarOpcionesAsync(EjemplarFormularioViewModel modelo)
        {
            modelo.LibrosDisponibles = await _contexto
                .Set<Libro>()
                .AsNoTracking()
                .Where(libro =>
                    libro.Activo ||
                    libro.LibroId == modelo.LibroId)
                .OrderBy(libro => libro.Titulo)
                .Select(libro => new SelectListItem
                {
                    Value = libro.LibroId.ToString(),
                    Text = libro.Titulo,
                    Selected = libro.LibroId == modelo.LibroId
                })
                .ToListAsync();

            bool incluirPrestado =
                modelo.Estado == EstadosEjemplar.Prestado;

            modelo.EstadosDisponibles = CrearOpcionesEstados(
                modelo.Estado,
                incluirPrestado);

            return modelo;
        }

        public async Task<List<EjemplarListadoViewModel>>
            ObtenerDisponiblesPorLibroAsync(int libroId)
        {
            return await _contexto
                .Set<Ejemplar>()
                .AsNoTracking()
                .Where(ejemplar =>
                    ejemplar.LibroId == libroId &&
                    ejemplar.Activo &&
                    ejemplar.Estado ==
                    EstadosEjemplar.Disponible &&
                    ejemplar.Libro.Activo)
                .OrderBy(ejemplar => ejemplar.CodigoInterno)
                .Select(ejemplar => new EjemplarListadoViewModel
                {
                    EjemplarId = ejemplar.EjemplarId,
                    LibroId = ejemplar.LibroId,
                    LibroTitulo = ejemplar.Libro.Titulo,
                    CodigoInterno = ejemplar.CodigoInterno,
                    Estado = ejemplar.Estado,
                    Ubicacion = ejemplar.Ubicacion,
                    FechaIngreso = ejemplar.FechaIngreso,
                    Activo = ejemplar.Activo,
                    TienePrestamoActivo = false
                })
                .ToListAsync();
        }

        public async Task<ResultadoOperacion> CrearAsync(
            EjemplarFormularioViewModel modelo)
        {
            string codigoLimpio = modelo.CodigoInterno.Trim();

            bool codigoRepetido = await _contexto
                .Set<Ejemplar>()
                .AnyAsync(ejemplar =>
                    ejemplar.CodigoInterno == codigoLimpio);

            if (codigoRepetido)
            {
                return ResultadoOperacion.Fallido(
                    "Ya existe un ejemplar con ese código interno.");
            }

            bool libroValido = await _contexto
                .Set<Libro>()
                .AnyAsync(libro =>
                    libro.LibroId == modelo.LibroId &&
                    libro.Activo);

            if (!libroValido)
            {
                return ResultadoOperacion.Fallido(
                    "El libro seleccionado no existe o está inactivo.");
            }

            if (!EstadosEjemplar
                .PuedeSeleccionarseManualmente(modelo.Estado))
            {
                return ResultadoOperacion.Fallido(
                    "El estado seleccionado no es válido para un ejemplar nuevo.");
            }

            var ejemplarNuevo = new Ejemplar
            {
                LibroId = modelo.LibroId,
                CodigoInterno = codigoLimpio,
                Estado = modelo.Estado,
                Ubicacion = LimpiarTextoOpcional(
                    modelo.Ubicacion),
                FechaIngreso = DateTime.Now,
                Activo = true
            };

            try
            {
                _contexto.Set<Ejemplar>().Add(ejemplarNuevo);
                await _contexto.SaveChangesAsync();

                return ResultadoOperacion.Correcto(
                    "El ejemplar se creó correctamente.");
            }
            catch (DbUpdateException excepcion)
            {
                _registro.LogError(
                    excepcion,
                    "Error al crear el ejemplar {CodigoInterno}.",
                    codigoLimpio);

                return ResultadoOperacion.Fallido(
                    "No fue posible guardar el ejemplar.");
            }
        }

        public async Task<ResultadoOperacion> EditarAsync(
            EjemplarFormularioViewModel modelo)
        {
            var ejemplar = await _contexto
                .Set<Ejemplar>()
                .FirstOrDefaultAsync(ejemplarActual =>
                    ejemplarActual.EjemplarId ==
                    modelo.EjemplarId);

            if (ejemplar is null)
            {
                return ResultadoOperacion.Fallido(
                    "El ejemplar que intentas editar no existe.");
            }

            string codigoLimpio = modelo.CodigoInterno.Trim();

            bool codigoRepetido = await _contexto
                .Set<Ejemplar>()
                .AnyAsync(ejemplarActual =>
                    ejemplarActual.CodigoInterno ==
                    codigoLimpio &&
                    ejemplarActual.EjemplarId !=
                    modelo.EjemplarId);

            if (codigoRepetido)
            {
                return ResultadoOperacion.Fallido(
                    "Ya existe otro ejemplar con ese código interno.");
            }

            bool libroValido = await _contexto
                .Set<Libro>()
                .AnyAsync(libro =>
                    libro.LibroId == modelo.LibroId &&
                    (libro.Activo ||
                     libro.LibroId == ejemplar.LibroId));

            if (!libroValido)
            {
                return ResultadoOperacion.Fallido(
                    "El libro seleccionado no existe o está inactivo.");
            }

            bool tienePrestamoActivo =
                await TienePrestamoActivoAsync(
                    modelo.EjemplarId);

            if (tienePrestamoActivo)
            {
                if (modelo.LibroId != ejemplar.LibroId)
                {
                    return ResultadoOperacion.Fallido(
                        "No puedes cambiar el libro de un ejemplar que está prestado.");
                }

                if (modelo.Estado != EstadosEjemplar.Prestado)
                {
                    return ResultadoOperacion.Fallido(
                        "No puedes cambiar el estado mientras el ejemplar tenga un préstamo activo.");
                }

                if (!modelo.Activo)
                {
                    return ResultadoOperacion.Fallido(
                        "No puedes desactivar un ejemplar que está prestado.");
                }
            }
            else if (modelo.Estado == EstadosEjemplar.Prestado)
            {
                return ResultadoOperacion.Fallido(
                    "El estado Prestado solo se asigna al registrar un préstamo.");
            }

            if (!EstadosEjemplar.EsValido(modelo.Estado))
            {
                return ResultadoOperacion.Fallido(
                    "El estado seleccionado no es válido.");
            }

            ejemplar.LibroId = modelo.LibroId;
            ejemplar.CodigoInterno = codigoLimpio;
            ejemplar.Estado = modelo.Estado;
            ejemplar.Ubicacion = LimpiarTextoOpcional(
                modelo.Ubicacion);
            ejemplar.Activo = modelo.Activo;

            try
            {
                await _contexto.SaveChangesAsync();

                return ResultadoOperacion.Correcto(
                    "El ejemplar se actualizó correctamente.");
            }
            catch (DbUpdateException excepcion)
            {
                _registro.LogError(
                    excepcion,
                    "Error al editar el ejemplar {EjemplarId}.",
                    modelo.EjemplarId);

                return ResultadoOperacion.Fallido(
                    "No fue posible actualizar el ejemplar.");
            }
        }

        public async Task<ResultadoOperacion> EliminarAsync(
            int ejemplarId)
        {
            var ejemplar = await _contexto
                .Set<Ejemplar>()
                .FirstOrDefaultAsync(ejemplarActual =>
                    ejemplarActual.EjemplarId == ejemplarId);

            if (ejemplar is null)
            {
                return ResultadoOperacion.Fallido(
                    "El ejemplar que intentas eliminar no existe.");
            }

            if (!ejemplar.Activo)
            {
                return ResultadoOperacion.Fallido(
                    "El ejemplar ya se encuentra inactivo.");
            }

            bool tienePrestamoActivo =
                await TienePrestamoActivoAsync(ejemplarId);

            if (tienePrestamoActivo)
            {
                return ResultadoOperacion.Fallido(
                    "No puedes eliminar un ejemplar que tiene un préstamo activo.");
            }

            // No lo borramos para conservar los préstamos anteriores.
            ejemplar.Activo = false;
            ejemplar.Estado = EstadosEjemplar.Baja;

            try
            {
                await _contexto.SaveChangesAsync();

                return ResultadoOperacion.Correcto(
                    "El ejemplar se desactivó correctamente.");
            }
            catch (DbUpdateException excepcion)
            {
                _registro.LogError(
                    excepcion,
                    "Error al desactivar el ejemplar {EjemplarId}.",
                    ejemplarId);

                return ResultadoOperacion.Fallido(
                    "No fue posible desactivar el ejemplar.");
            }
        }

        private async Task<bool> TienePrestamoActivoAsync(
            int ejemplarId)
        {
            return await _contexto
                .Set<Prestamo>()
                .AsNoTracking()
                .AnyAsync(prestamo =>
                    prestamo.EjemplarId == ejemplarId &&
                    (
    prestamo.Estado == EstadosPrestamo.Activo ||
    prestamo.Estado == EstadosPrestamo.Vencido
));
        }

        private async Task<List<SelectListItem>>
            ObtenerOpcionesLibrosAsync(int? libroId)
        {
            return await _contexto
                .Set<Libro>()
                .AsNoTracking()
                .OrderBy(libro => libro.Titulo)
                .Select(libro => new SelectListItem
                {
                    Value = libro.LibroId.ToString(),
                    Text = libro.Titulo,
                    Selected =
                        libroId.HasValue &&
                        libro.LibroId == libroId.Value
                })
                .ToListAsync();
        }

        private static List<SelectListItem>
            CrearOpcionesEstados(
                string? estadoSeleccionado,
                bool incluirPrestado)
        {
            var estados = new List<string>
            {
                EstadosEjemplar.Disponible,
                EstadosEjemplar.Danado,
                EstadosEjemplar.Baja
            };

            if (incluirPrestado)
            {
                estados.Insert(
                    1,
                    EstadosEjemplar.Prestado);
            }

            return estados
                .Select(estado => new SelectListItem
                {
                    Value = estado,
                    Text = estado,
                    Selected =
                        estado == estadoSeleccionado
                })
                .ToList();
        }

        private static string? LimpiarTextoOpcional(
            string? texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
            {
                return null;
            }

            return texto.Trim();
        }
    }
}