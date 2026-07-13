using BookCore.Services;
using BookCore.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace BookCore.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class LibrosController : Controller
    {
        private readonly ILibroServicio _libroServicio;

        public LibrosController(
            ILibroServicio libroServicio)
        {
            _libroServicio = libroServicio;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            string? busqueda,
            int? categoriaId,
            bool soloActivos = false)
        {
            var modelo = await _libroServicio.ObtenerIndiceAsync(
                busqueda,
                categoriaId,
                soloActivos);

            return View(modelo);
        }

        [HttpGet]
        public async Task<IActionResult> Detalle(int id)
        {
            var libro = await _libroServicio
                .ObtenerDetalleAsync(id);

            if (libro is null)
            {
                return NotFound();
            }

            return View(libro);
        }

        [HttpGet]
        public async Task<IActionResult> Crear()
        {
            var modelo = await _libroServicio
                .PrepararCreacionAsync();

            return View(modelo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(
            LibroFormularioViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                modelo = await _libroServicio
                    .CargarOpcionesAsync(modelo);

                return View(modelo);
            }

            var resultado = await _libroServicio
                .CrearAsync(modelo);

            if (!resultado.Exitoso)
            {
                ModelState.AddModelError(
                    string.Empty,
                    resultado.Mensaje);

                modelo = await _libroServicio
                    .CargarOpcionesAsync(modelo);

                return View(modelo);
            }

            TempData["MensajeExito"] = resultado.Mensaje;

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var modelo = await _libroServicio
                .PrepararEdicionAsync(id);

            if (modelo is null)
            {
                return NotFound();
            }

            return View(modelo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(
            int id,
            LibroFormularioViewModel modelo)
        {
            if (id != modelo.LibroId)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                modelo = await _libroServicio
                    .CargarOpcionesAsync(modelo);

                return View(modelo);
            }

            var resultado = await _libroServicio
                .EditarAsync(modelo);

            if (!resultado.Exitoso)
            {
                ModelState.AddModelError(
                    string.Empty,
                    resultado.Mensaje);

                modelo = await _libroServicio
                    .CargarOpcionesAsync(modelo);

                return View(modelo);
            }

            TempData["MensajeExito"] = resultado.Mensaje;

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Eliminar(int id)
        {
            var libro = await _libroServicio
                .ObtenerDetalleAsync(id);

            if (libro is null)
            {
                return NotFound();
            }

            return View(libro);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarEliminacion(
            int libroId)
        {
            var resultado = await _libroServicio
                .EliminarAsync(libroId);

            if (resultado.Exitoso)
            {
                TempData["MensajeExito"] = resultado.Mensaje;
            }
            else
            {
                TempData["MensajeError"] = resultado.Mensaje;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}