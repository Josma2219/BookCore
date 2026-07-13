using BookCore.Services;
using BookCore.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BookCore.Controllers
{
    // Este controlador se protege cuando agreguemos el login.
    public class PrestamosController : Controller
    {
        private readonly IPrestamoServicio _prestamoServicio;

        public PrestamosController(
            IPrestamoServicio prestamoServicio)
        {
            _prestamoServicio = prestamoServicio;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            string? busqueda,
            string? estado,
            DateTime? fechaDesde,
            DateTime? fechaHasta)
        {
            var modelo = await _prestamoServicio
                .ObtenerIndiceAsync(
                    busqueda,
                    estado,
                    fechaDesde,
                    fechaHasta);

            return View(modelo);
        }

        [HttpGet]
        public async Task<IActionResult> Detalle(int id)
        {
            var prestamo = await _prestamoServicio
                .ObtenerDetalleAsync(id);

            if (prestamo is null)
            {
                return NotFound();
            }

            return View(prestamo);
        }

        [HttpGet]
        public async Task<IActionResult> Crear(
            int? usuarioBibliotecaId,
            int? libroId)
        {
            var modelo = await _prestamoServicio
                .PrepararCreacionAsync(
                    usuarioBibliotecaId,
                    libroId);

            return View(modelo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(
            PrestamoFormularioViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                modelo = await _prestamoServicio
                    .CargarOpcionesAsync(modelo);

                return View(modelo);
            }

            var resultado = await _prestamoServicio
                .CrearAsync(modelo);

            if (!resultado.Exitoso)
            {
                ModelState.AddModelError(
                    string.Empty,
                    resultado.Mensaje);

                modelo = await _prestamoServicio
                    .CargarOpcionesAsync(modelo);

                return View(modelo);
            }

            TempData["MensajeExito"] = resultado.Mensaje;

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult>
            ObtenerEjemplaresDisponibles(int libroId)
        {
            if (libroId <= 0)
            {
                return Json(Array.Empty<object>());
            }

            var ejemplares = await _prestamoServicio
                .ObtenerEjemplaresDisponiblesAsync(libroId);

            return Json(
                ejemplares.Select(ejemplar => new
                {
                    valor = ejemplar.EjemplarId,
                    texto = ejemplar.Texto
                }));
        }

        [HttpGet]
        public async Task<IActionResult> Devolver(int id)
        {
            var prestamo = await _prestamoServicio
                .ObtenerDetalleAsync(id);

            if (prestamo is null)
            {
                return NotFound();
            }

            if (!prestamo.PuedeDevolverse)
            {
                TempData["MensajeError"] =
                    "Este préstamo ya fue devuelto.";

                return RedirectToAction(nameof(Index));
            }

            return View(prestamo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>
            ConfirmarDevolucion(int prestamoId)
        {
            var resultado = await _prestamoServicio
                .DevolverAsync(prestamoId);

            if (resultado.Exitoso)
            {
                TempData["MensajeExito"] =
                    resultado.Mensaje;
            }
            else
            {
                TempData["MensajeError"] =
                    resultado.Mensaje;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}