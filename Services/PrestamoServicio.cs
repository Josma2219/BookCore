using BookCore.Data;
using BookCore.Helpers;
using BookCore.Models.Entidades;
using BookCore.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BookCore.Services
{
    public class PrestamoServicio : IPrestamoServicio
    {
        private readonly BookCoreContexto _contexto;
        private readonly ILogger<PrestamoServicio> _registro;

        public PrestamoServicio(
            BookCoreContexto contexto,
            ILogger<PrestamoServicio> registro)
        {
            _contexto = contexto;
            _registro = registro;
        }

        public async Task<PrestamoIndiceViewModel> ObtenerIndiceAsync(
            string? busqueda,
            string? estado,
            DateTime? fechaDesde,
            DateTime? fechaHasta)
        {
            await ActualizarEstadosVencidosAsync();

            var consulta =
                from prestamo in _contexto
                    .Set<Prestamo>()
                    .AsNoTracking()

                join usuario in _contexto
                    .Set<UsuarioBiblioteca>()
                    .AsNoTracking()
                    on prestamo.UsuarioBibliotecaId
                    equals usuario.UsuarioBibliotecaId

                join ejemplar in _contexto
                    .Set<Ejemplar>()
                    .AsNoTracking()
                    on prestamo.EjemplarId
                    equals ejemplar.EjemplarId

                join libro in _contexto
                    .Set<Libro>()
                    .AsNoTracking()
                    on ejemplar.LibroId
                    equals libro.LibroId

                select new
                {
                    Prestamo = prestamo,
                    Usuario = usuario,
                    Ejemplar = ejemplar,
                    Libro = libro
                };

            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                string textoBusqueda = busqueda.Trim();

                consulta = consulta.Where(registro =>
                    registro.Usuario.Nombre.Contains(textoBusqueda) ||
                    registro.Usuario.Apellidos.Contains(textoBusqueda) ||
                    registro.Usuario.Cedula.Contains(textoBusqueda) ||
                    registro.Libro.Titulo.Contains(textoBusqueda) ||
                    registro.Ejemplar.CodigoInterno.Contains(textoBusqueda));
            }

            if (!string.IsNullOrWhiteSpace(estado))
            {
                consulta = consulta.Where(registro =>
                    registro.Prestamo.Estado == estado);
            }

            if (fechaDesde.HasValue)
            {
                DateTime inicio = fechaDesde.Value.Date;

                consulta = consulta.Where(registro =>
                    registro.Prestamo.FechaPrestamo >= inicio);
            }

            if (fechaHasta.HasValue)
            {
                DateTime finalExclusivo =
                    fechaHasta.Value.Date.AddDays(1);

                consulta = consulta.Where(registro =>
                    registro.Prestamo.FechaPrestamo <
                    finalExclusivo);
            }

            var prestamos = await consulta
                .OrderByDescending(registro =>
                    registro.Prestamo.FechaPrestamo)
                .Select(registro =>
                    new PrestamoListadoViewModel
                    {
                        PrestamoId =
                            registro.Prestamo.PrestamoId,

                        UsuarioNombreCompleto =
                            registro.Usuario.Nombre + " " +
                            registro.Usuario.Apellidos,

                        UsuarioCedula =
                            registro.Usuario.Cedula,

                        LibroTitulo =
                            registro.Libro.Titulo,

                        EjemplarCodigo =
                            registro.Ejemplar.CodigoInterno,

                        FechaPrestamo =
                            registro.Prestamo.FechaPrestamo,

                        FechaVencimiento =
                            registro.Prestamo.FechaVencimiento,

                        FechaDevolucion =
                            registro.Prestamo.FechaDevolucion,

                        Estado =
                            registro.Prestamo.Estado
                    })
                .ToListAsync();

            return new PrestamoIndiceViewModel
            {
                Busqueda = busqueda,
                Estado = estado,
                FechaDesde = fechaDesde,
                FechaHasta = fechaHasta,
                Prestamos = prestamos,
                Estados = CrearOpcionesEstados(estado)
            };
        }

        public async Task<PrestamoDetalleViewModel?>
            ObtenerDetalleAsync(int prestamoId)
        {
            await ActualizarEstadosVencidosAsync();

            return await (
                from prestamo in _contexto
                    .Set<Prestamo>()
                    .AsNoTracking()

                join usuario in _contexto
                    .Set<UsuarioBiblioteca>()
                    .AsNoTracking()
                    on prestamo.UsuarioBibliotecaId
                    equals usuario.UsuarioBibliotecaId

                join ejemplar in _contexto
                    .Set<Ejemplar>()
                    .AsNoTracking()
                    on prestamo.EjemplarId
                    equals ejemplar.EjemplarId

                join libro in _contexto
                    .Set<Libro>()
                    .AsNoTracking()
                    on ejemplar.LibroId
                    equals libro.LibroId

                where prestamo.PrestamoId == prestamoId

                select new PrestamoDetalleViewModel
                {
                    PrestamoId = prestamo.PrestamoId,

                    UsuarioBibliotecaId =
                        usuario.UsuarioBibliotecaId,

                    LibroId = libro.LibroId,

                    EjemplarId = ejemplar.EjemplarId,

                    UsuarioNombreCompleto =
                        usuario.Nombre + " " +
                        usuario.Apellidos,

                    UsuarioCedula = usuario.Cedula,

                    UsuarioCorreo = usuario.Correo,

                    LibroTitulo = libro.Titulo,

                    EjemplarCodigo =
                        ejemplar.CodigoInterno,

                    EjemplarUbicacion =
                        ejemplar.Ubicacion,

                    FechaPrestamo =
                        prestamo.FechaPrestamo,

                    FechaVencimiento =
                        prestamo.FechaVencimiento,

                    FechaDevolucion =
                        prestamo.FechaDevolucion,

                    Estado = prestamo.Estado,

                    Observaciones =
                        prestamo.Observaciones
                })
                .FirstOrDefaultAsync();
        }

        public async Task<PrestamoFormularioViewModel>
            PrepararCreacionAsync(
                int? usuarioBibliotecaId = null,
                int? libroId = null)
        {
            var modelo = new PrestamoFormularioViewModel
            {
                UsuarioBibliotecaId =
                    usuarioBibliotecaId ?? 0,

                LibroId = libroId ?? 0,

                FechaVencimiento =
                    DateTime.Today.AddDays(7)
            };

            return await CargarOpcionesAsync(modelo);
        }

        public async Task<PrestamoFormularioViewModel>
            CargarOpcionesAsync(
                PrestamoFormularioViewModel modelo)
        {
            modelo.UsuariosDisponibles = await _contexto
                .Set<UsuarioBiblioteca>()
                .AsNoTracking()
                .Where(usuario =>
                    usuario.Activo ||
                    usuario.UsuarioBibliotecaId ==
                    modelo.UsuarioBibliotecaId)
                .OrderBy(usuario => usuario.Apellidos)
                .ThenBy(usuario => usuario.Nombre)
                .Select(usuario => new SelectListItem
                {
                    Value =
                        usuario.UsuarioBibliotecaId.ToString(),

                    Text =
                        usuario.Nombre + " " +
                        usuario.Apellidos + " — " +
                        usuario.Cedula,

                    Selected =
                        usuario.UsuarioBibliotecaId ==
                        modelo.UsuarioBibliotecaId
                })
                .ToListAsync();

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

                    Selected =
                        libro.LibroId ==
                        modelo.LibroId
                })
                .ToListAsync();

            if (modelo.LibroId > 0)
            {
                var ejemplares =
                    await ObtenerEjemplaresDisponiblesAsync(
                        modelo.LibroId);

                modelo.EjemplaresDisponibles = ejemplares
                    .Select(ejemplar =>
                        new SelectListItem
                        {
                            Value =
                                ejemplar.EjemplarId.ToString(),

                            Text = ejemplar.Texto,

                            Selected =
                                ejemplar.EjemplarId ==
                                modelo.EjemplarId
                        })
                    .ToList();
            }

            return modelo;
        }

        public async Task<List<OpcionEjemplarViewModel>>
            ObtenerEjemplaresDisponiblesAsync(int libroId)
        {
            return await _contexto
                .Set<Ejemplar>()
                .AsNoTracking()
                .Where(ejemplar =>
                    ejemplar.LibroId == libroId &&
                    ejemplar.Activo &&
                    ejemplar.Estado ==
                    EstadosEjemplar.Disponible &&
                    !_contexto.Set<Prestamo>().Any(
                        prestamo =>
                            prestamo.EjemplarId ==
                            ejemplar.EjemplarId &&
                            (
                                prestamo.Estado ==
                                EstadosPrestamo.Activo ||
                                prestamo.Estado ==
                                EstadosPrestamo.Vencido
                            )))
                .OrderBy(ejemplar =>
                    ejemplar.CodigoInterno)
                .Select(ejemplar =>
                    new OpcionEjemplarViewModel
                    {
                        EjemplarId =
                            ejemplar.EjemplarId,

                        Texto =
                            ejemplar.CodigoInterno +
                            (
                                ejemplar.Ubicacion == null
                                    ? ""
                                    : " — " +
                                      ejemplar.Ubicacion
                            )
                    })
                .ToListAsync();
        }

        public async Task<ResultadoOperacion> CrearAsync(
            PrestamoFormularioViewModel modelo)
        {
            if (modelo.FechaVencimiento.Date <
                DateTime.Today)
            {
                return ResultadoOperacion.Fallido(
                    "La fecha de vencimiento no puede ser anterior a hoy.");
            }

            bool usuarioValido = await _contexto
                .Set<UsuarioBiblioteca>()
                .AnyAsync(usuario =>
                    usuario.UsuarioBibliotecaId ==
                    modelo.UsuarioBibliotecaId &&
                    usuario.Activo);

            if (!usuarioValido)
            {
                return ResultadoOperacion.Fallido(
                    "El usuario seleccionado no existe o está inactivo.");
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

            await using var transaccion =
                await _contexto.Database
                    .BeginTransactionAsync();

            try
            {
                var ejemplar = await _contexto
                    .Set<Ejemplar>()
                    .FirstOrDefaultAsync(ejemplarActual =>
                        ejemplarActual.EjemplarId ==
                        modelo.EjemplarId);

                if (ejemplar is null ||
                    !ejemplar.Activo ||
                    ejemplar.LibroId != modelo.LibroId ||
                    ejemplar.Estado !=
                    EstadosEjemplar.Disponible)
                {
                    return ResultadoOperacion.Fallido(
                        "El ejemplar seleccionado ya no está disponible.");
                }

                bool tienePrestamoPendiente =
                    await _contexto
                        .Set<Prestamo>()
                        .AnyAsync(prestamo =>
                            prestamo.EjemplarId ==
                            modelo.EjemplarId &&
                            (
                                prestamo.Estado ==
                                EstadosPrestamo.Activo ||
                                prestamo.Estado ==
                                EstadosPrestamo.Vencido
                            ));

                if (tienePrestamoPendiente)
                {
                    return ResultadoOperacion.Fallido(
                        "El ejemplar ya tiene un préstamo pendiente.");
                }

                var prestamoNuevo = new Prestamo
                {
                    UsuarioBibliotecaId =
                        modelo.UsuarioBibliotecaId,

                    EjemplarId = modelo.EjemplarId,

                    FechaPrestamo = DateTime.Now,

                    FechaVencimiento =
                        modelo.FechaVencimiento.Date,

                    FechaDevolucion = null,

                    Estado = EstadosPrestamo.Activo,

                    Observaciones =
                        LimpiarTextoOpcional(
                            modelo.Observaciones)
                };

                ejemplar.Estado =
                    EstadosEjemplar.Prestado;

                _contexto
                    .Set<Prestamo>()
                    .Add(prestamoNuevo);

                await _contexto.SaveChangesAsync();

                await transaccion.CommitAsync();

                return ResultadoOperacion.Correcto(
                    "El préstamo se registró correctamente.");
            }
            catch (Exception excepcion)
            {
                await transaccion.RollbackAsync();

                _registro.LogError(
                    excepcion,
                    "Error al registrar el préstamo del ejemplar {EjemplarId}.",
                    modelo.EjemplarId);

                return ResultadoOperacion.Fallido(
                    "No fue posible registrar el préstamo.");
            }
        }

        public async Task<ResultadoOperacion> DevolverAsync(
            int prestamoId)
        {
            await using var transaccion =
                await _contexto.Database
                    .BeginTransactionAsync();

            try
            {
                var prestamo = await _contexto
                    .Set<Prestamo>()
                    .FirstOrDefaultAsync(prestamoActual =>
                        prestamoActual.PrestamoId ==
                        prestamoId);

                if (prestamo is null)
                {
                    return ResultadoOperacion.Fallido(
                        "El préstamo que intentas devolver no existe.");
                }

                if (!EstadosPrestamo.EsPendiente(
                    prestamo.Estado))
                {
                    return ResultadoOperacion.Fallido(
                        "Este préstamo ya fue devuelto.");
                }

                var ejemplar = await _contexto
                    .Set<Ejemplar>()
                    .FirstOrDefaultAsync(ejemplarActual =>
                        ejemplarActual.EjemplarId ==
                        prestamo.EjemplarId);

                if (ejemplar is null)
                {
                    return ResultadoOperacion.Fallido(
                        "No se encontró el ejemplar asociado al préstamo.");
                }

                prestamo.FechaDevolucion = DateTime.Now;
                prestamo.Estado =
                    EstadosPrestamo.Devuelto;

                ejemplar.Estado =
                    EstadosEjemplar.Disponible;

                await _contexto.SaveChangesAsync();

                await transaccion.CommitAsync();

                return ResultadoOperacion.Correcto(
                    "La devolución se registró correctamente.");
            }
            catch (Exception excepcion)
            {
                await transaccion.RollbackAsync();

                _registro.LogError(
                    excepcion,
                    "Error al devolver el préstamo {PrestamoId}.",
                    prestamoId);

                return ResultadoOperacion.Fallido(
                    "No fue posible registrar la devolución.");
            }
        }

        private async Task ActualizarEstadosVencidosAsync()
        {
            DateTime hoy = DateTime.Today;

            var prestamosVencidos = await _contexto
                .Set<Prestamo>()
                .Where(prestamo =>
                    prestamo.Estado ==
                    EstadosPrestamo.Activo &&
                    prestamo.FechaVencimiento < hoy)
                .ToListAsync();

            if (prestamosVencidos.Count == 0)
            {
                return;
            }

            foreach (var prestamo in prestamosVencidos)
            {
                prestamo.Estado =
                    EstadosPrestamo.Vencido;
            }

            await _contexto.SaveChangesAsync();
        }

        private static List<SelectListItem>
            CrearOpcionesEstados(
                string? estadoSeleccionado)
        {
            var estados = new[]
            {
                EstadosPrestamo.Activo,
                EstadosPrestamo.Vencido,
                EstadosPrestamo.Devuelto
            };

            return estados
                .Select(estado =>
                    new SelectListItem
                    {
                        Value = estado,
                        Text = estado,
                        Selected =
                            estado ==
                            estadoSeleccionado
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