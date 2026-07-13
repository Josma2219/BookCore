using BookCore.Services;
using BookCore.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace BookCore.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class CategoriasController : Controller
    {
        private readonly ICategoriaServicio _categoriaServicio;

        public CategoriasController(
            ICategoriaServicio categoriaServicio)
        {
            _categoriaServicio = categoriaServicio;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var categorias = await _categoriaServicio
                .ObtenerTodasAsync();

            return View(categorias);
        }

        [HttpGet]
        public async Task<IActionResult> Detalle(int id)
        {
            var categoria = await _categoriaServicio
                .ObtenerPorIdAsync(id);

            if (categoria is null)
            {
                return NotFound();
            }

            return View(categoria);
        }

        [HttpGet]
        public IActionResult Crear()
        {
            return View(new CategoriaViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(
            CategoriaViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            var resultado = await _categoriaServicio
                .CrearAsync(modelo);

            if (!resultado.Exitoso)
            {
                ModelState.AddModelError(
                    nameof(modelo.Nombre),
                    resultado.Mensaje);

                return View(modelo);
            }

            TempData["MensajeExito"] = resultado.Mensaje;

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var categoria = await _categoriaServicio
                .ObtenerPorIdAsync(id);

            if (categoria is null)
            {
                return NotFound();
            }

            return View(categoria);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(
            int id,
            CategoriaViewModel modelo)
        {
            if (id != modelo.CategoriaId)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            var resultado = await _categoriaServicio
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
            var categoria = await _categoriaServicio
                .ObtenerPorIdAsync(id);

            if (categoria is null)
            {
                return NotFound();
            }

            return View(categoria);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarEliminacion(
            int categoriaId)
        {
            var resultado = await _categoriaServicio
                .EliminarAsync(categoriaId);

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