using BookCore.Data;
using BookCore.Helpers;
using BookCore.Models.Entidades;
using BookCore.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace BookCore.Services
{
    public class MiCuentaServicio
        : IMiCuentaServicio
    {
        private readonly BookCoreContexto _contexto;

        public MiCuentaServicio(
            BookCoreContexto contexto)
        {
            _contexto = contexto;
        }

        public async Task<MiCuentaViewModel?>
            ObtenerResumenAsync(
                int usuarioBibliotecaId)
        {
            var usuario = await _contexto
                .Set<UsuarioBiblioteca>()
                .AsNoTracking()
                .FirstOrDefaultAsync(usuarioActual =>
                    usuarioActual.UsuarioBibliotecaId ==
                    usuarioBibliotecaId &&
                    usuarioActual.Activo);

            if (usuario is null)
            {
                return null;
            }
            var prestamosActuales =
                await ConsultarPrestamos(
                        usuarioBibliotecaId)
                    .Where(prestamo =>
                        prestamo.Estado ==
                            EstadosPrestamo.Activo ||
                        prestamo.Estado ==
                            EstadosPrestamo.Vencido)
                                .OrderBy(prestamo =>
                        prestamo.FechaVencimiento)
                    .ToListAsync();

            int totalPrestamos = await _contexto
                .Set<Prestamo>()
                .AsNoTracking()
                .CountAsync(prestamo =>
                    prestamo.UsuarioBibliotecaId ==
                    usuarioBibliotecaId);

            int totalFavoritos = await _contexto
                .Set<Favorito>()
                .AsNoTracking()
                .CountAsync(favorito =>
                    favorito.UsuarioBibliotecaId ==
                    usuarioBibliotecaId);

            return new MiCuentaViewModel
            {
                UsuarioBibliotecaId =
                    usuario.UsuarioBibliotecaId,

                NombreCompleto =
                    usuario.Nombre + " " +
                    usuario.Apellidos,

                Cedula =
                    usuario.Cedula,

                Correo =
                    usuario.Correo,

                Telefono =
                    usuario.Telefono,

                TotalPrestamosHistoricos =
                    totalPrestamos,

                TotalFavoritos =
                    totalFavoritos,

                PrestamosActuales =
                    prestamosActuales
            };
        }

        public async Task<HistorialUsuarioViewModel?>
            ObtenerHistorialAsync(
                int usuarioBibliotecaId)
        {
            var usuario = await _contexto
                .Set<UsuarioBiblioteca>()
                .AsNoTracking()
                .FirstOrDefaultAsync(usuarioActual =>
                    usuarioActual.UsuarioBibliotecaId ==
                    usuarioBibliotecaId &&
                    usuarioActual.Activo);

            if (usuario is null)
            {
                return null;
            }

            var prestamos =
                await ConsultarPrestamos(
                        usuarioBibliotecaId)
                    .OrderByDescending(prestamo =>
                        prestamo.FechaPrestamo)
                    .ToListAsync();

            return new HistorialUsuarioViewModel
            {
                NombreCompleto =
                    usuario.Nombre + " " +
                    usuario.Apellidos,

                Prestamos =
                    prestamos
            };
        }

        private IQueryable<PrestamoUsuarioViewModel>
            ConsultarPrestamos(
                int usuarioBibliotecaId)
        {
            DateTime hoy = DateTime.Today;

            return
                from prestamo in _contexto
                    .Set<Prestamo>()
                    .AsNoTracking()

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

                where prestamo.UsuarioBibliotecaId ==
                      usuarioBibliotecaId

                select new PrestamoUsuarioViewModel
                {
                    PrestamoId =
                        prestamo.PrestamoId,

                    LibroId =
                        libro.LibroId,

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
                };
        }
    }
}