using BookCore.Services;
using BookCore.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace BookCore.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class AutoresController : Controller
    {
        private readonly IAutorServicio _autorServicio;

        public AutoresController(
            IAutorServicio autorServicio)
        {
            _autorServicio = autorServicio;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var autores = await _autorServicio
                .ObtenerTodosAsync();

            return View(autores);
        }

        [HttpGet]
        public async Task<IActionResult> Detalle(int id)
        {
            var autor = await _autorServicio
                .ObtenerPorIdAsync(id);

            if (autor is null)
            {
                return NotFound();
            }

            return View(autor);
        }

        [HttpGet]
        public IActionResult Crear()
        {
            return View(new AutorViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(
            AutorViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            var resultado = await _autorServicio
                .CrearAsync(modelo);

            if (!resultado.Exitoso)
            {
                ModelState.AddModelError(
                    string.Empty,
                    resultado.Mensaje);

                return View(modelo);
            }

            TempData["MensajeExito"] = resultado.Mensaje;

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var autor = await _autorServicio
                .ObtenerPorIdAsync(id);

            if (autor is null)
            {
                return NotFound();
            }

            return View(autor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(
            int id,
            AutorViewModel modelo)
        {
            if (id != modelo.AutorId)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            var resultado = await _autorServicio
                .EditarAsync(modelo);

            if (!resultado.Exitoso)
            {
                ModelState.AddModelError(
                    string.Empty,
                    resultado.Mensaje);

                return View(modelo);
            }

            TempData["MensajeExito"] = resultado.Mensaje;

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Eliminar(int id)
        {
            var autor = await _autorServicio
                .ObtenerPorIdAsync(id);

            if (autor is null)
            {
                return NotFound();
            }

            return View(autor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarEliminacion(
            int autorId)
        {
            var resultado = await _autorServicio
                .EliminarAsync(autorId);

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