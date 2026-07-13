using BookCore.Services;
using BookCore.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace BookCore.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class EjemplaresController : Controller
    {
        private readonly IEjemplarServicio _ejemplarServicio;

        public EjemplaresController(
            IEjemplarServicio ejemplarServicio)
        {
            _ejemplarServicio = ejemplarServicio;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            string? busqueda,
            int? libroId,
            string? estado,
            bool soloActivos = false)
        {
            var modelo = await _ejemplarServicio
                .ObtenerIndiceAsync(
                    busqueda,
                    libroId,
                    estado,
                    soloActivos);

            return View(modelo);
        }

        [HttpGet]
        public async Task<IActionResult> Detalle(int id)
        {
            var ejemplar = await _ejemplarServicio
                .ObtenerPorIdAsync(id);

            if (ejemplar is null)
            {
                return NotFound();
            }

            return View(ejemplar);
        }

        [HttpGet]
        public async Task<IActionResult> Crear(int? libroId)
        {
            var modelo = await _ejemplarServicio
                .PrepararCreacionAsync(libroId);

            return View(modelo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(
            EjemplarFormularioViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                modelo = await _ejemplarServicio
                    .CargarOpcionesAsync(modelo);

                return View(modelo);
            }

            var resultado = await _ejemplarServicio
                .CrearAsync(modelo);

            if (!resultado.Exitoso)
            {
                ModelState.AddModelError(
                    string.Empty,
                    resultado.Mensaje);

                modelo = await _ejemplarServicio
                    .CargarOpcionesAsync(modelo);

                return View(modelo);
            }

            TempData["MensajeExito"] = resultado.Mensaje;

            return RedirectToAction(
                nameof(Index),
                new { libroId = modelo.LibroId });
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var modelo = await _ejemplarServicio
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
            EjemplarFormularioViewModel modelo)
        {
            if (id != modelo.EjemplarId)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                modelo = await _ejemplarServicio
                    .CargarOpcionesAsync(modelo);

                return View(modelo);
            }

            var resultado = await _ejemplarServicio
                .EditarAsync(modelo);

            if (!resultado.Exitoso)
            {
                ModelState.AddModelError(
                    string.Empty,
                    resultado.Mensaje);

                modelo = await _ejemplarServicio
                    .CargarOpcionesAsync(modelo);

                return View(modelo);
            }

            TempData["MensajeExito"] = resultado.Mensaje;

            return RedirectToAction(
                nameof(Index),
                new { libroId = modelo.LibroId });
        }

        [HttpGet]
        public async Task<IActionResult> Eliminar(int id)
        {
            var ejemplar = await _ejemplarServicio
                .ObtenerPorIdAsync(id);

            if (ejemplar is null)
            {
                return NotFound();
            }

            return View(ejemplar);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarEliminacion(
            int ejemplarId)
        {
            var ejemplar = await _ejemplarServicio
                .ObtenerPorIdAsync(ejemplarId);

            if (ejemplar is null)
            {
                return NotFound();
            }

            var resultado = await _ejemplarServicio
                .EliminarAsync(ejemplarId);

            if (resultado.Exitoso)
            {
                TempData["MensajeExito"] = resultado.Mensaje;
            }
            else
            {
                TempData["MensajeError"] = resultado.Mensaje;
            }

            return RedirectToAction(
                nameof(Index),
                new { libroId = ejemplar.LibroId });
        }
    }
}