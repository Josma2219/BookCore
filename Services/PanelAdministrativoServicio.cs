using BookCore.Data;
using BookCore.Helpers;
using BookCore.Models.Entidades;
using BookCore.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace BookCore.Services
{
    public class PanelAdministrativoServicio
        : IPanelAdministrativoServicio
    {
        private readonly BookCoreContexto _contexto;

        public PanelAdministrativoServicio(
            BookCoreContexto contexto)
        {
            _contexto = contexto;
        }

        public async Task<PanelAdministrativoViewModel>
            ObtenerPanelAsync()
        {
            DateTime hoy = DateTime.Today;
            DateTime manana = hoy.AddDays(1);

            int totalLibrosActivos = await _contexto
                .Set<Libro>()
                .AsNoTracking()
                .CountAsync(libro => libro.Activo);

            int totalUsuariosActivos = await _contexto
                .Set<UsuarioBiblioteca>()
                .AsNoTracking()
                .CountAsync(usuario => usuario.Activo);

            int totalEjemplaresActivos = await _contexto
                .Set<Ejemplar>()
                .AsNoTracking()
                .CountAsync(ejemplar => ejemplar.Activo);

            int ejemplaresDisponibles = await _contexto
                .Set<Ejemplar>()
                .AsNoTracking()
                .CountAsync(ejemplar =>
                    ejemplar.Activo &&
                    ejemplar.Estado ==
                    EstadosEjemplar.Disponible);

            int prestamosPendientes = await _contexto
                .Set<Prestamo>()
                .AsNoTracking()
                .CountAsync(prestamo =>
                    prestamo.Estado ==
                    EstadosPrestamo.Activo ||
                    prestamo.Estado ==
                    EstadosPrestamo.Vencido);

            int prestamosVencidos = await _contexto
                .Set<Prestamo>()
                .AsNoTracking()
                .CountAsync(prestamo =>
                    prestamo.Estado ==
                    EstadosPrestamo.Vencido ||
                    (
                        prestamo.Estado ==
                        EstadosPrestamo.Activo &&
                        prestamo.FechaVencimiento < hoy
                    ));

            int prestamosVencenHoy = await _contexto
                .Set<Prestamo>()
                .AsNoTracking()
                .CountAsync(prestamo =>
                    (
                        prestamo.Estado ==
                        EstadosPrestamo.Activo ||
                        prestamo.Estado ==
                        EstadosPrestamo.Vencido
                    ) &&
                    prestamo.FechaVencimiento >= hoy &&
                    prestamo.FechaVencimiento < manana);

            var prestamosRecientes = await (
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

                orderby prestamo.FechaPrestamo descending

                select new PrestamoListadoViewModel
                {
                    PrestamoId = prestamo.PrestamoId,

                    UsuarioNombreCompleto =
                        usuario.Nombre + " " +
                        usuario.Apellidos,

                    UsuarioCedula =
                        usuario.Cedula,

                    LibroTitulo =
                        libro.Titulo,

                    EjemplarCodigo =
                        ejemplar.CodigoInterno,

                    FechaPrestamo =
                        prestamo.FechaPrestamo,

                    FechaVencimiento =
                        prestamo.FechaVencimiento,

                    FechaDevolucion =
                        prestamo.FechaDevolucion,

                    Estado =
                        prestamo.Estado ==
                            EstadosPrestamo.Activo &&
                        prestamo.FechaVencimiento < hoy

                            ? EstadosPrestamo.Vencido
                            : prestamo.Estado
                })
                .Take(5)
                .ToListAsync();

            return new PanelAdministrativoViewModel
            {
                TotalLibrosActivos =
                    totalLibrosActivos,

                TotalUsuariosActivos =
                    totalUsuariosActivos,

                TotalEjemplaresActivos =
                    totalEjemplaresActivos,

                EjemplaresDisponibles =
                    ejemplaresDisponibles,

                PrestamosPendientes =
                    prestamosPendientes,

                PrestamosVencidos =
                    prestamosVencidos,

                PrestamosVencenHoy =
                    prestamosVencenHoy,

                PrestamosRecientes =
                    prestamosRecientes
            };
        }
    }
}