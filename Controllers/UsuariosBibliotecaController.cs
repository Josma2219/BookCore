using BookCore.Services;
using BookCore.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BookCore.Controllers
{
    // Luego este controlador se protege con el login administrativo.
    public class UsuariosBibliotecaController : Controller
    {
        private readonly IUsuarioBibliotecaServicio _usuarioServicio;

        public UsuariosBibliotecaController(
            IUsuarioBibliotecaServicio usuarioServicio)
        {
            _usuarioServicio = usuarioServicio;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? busqueda)
        {
            var usuarios = await _usuarioServicio
                .ObtenerTodosAsync(busqueda);

            ViewBag.Busqueda = busqueda;

            return View(usuarios);
        }

        [HttpGet]
        public async Task<IActionResult> Detalle(int id)
        {
            var usuario = await _usuarioServicio
                .ObtenerPorIdAsync(id);

            if (usuario is null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        [HttpGet]
        public IActionResult Crear()
        {
            return View(new UsuarioBibliotecaViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(
            UsuarioBibliotecaViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            var resultado = await _usuarioServicio
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
            var usuario = await _usuarioServicio
                .ObtenerPorIdAsync(id);

            if (usuario is null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(
            int id,
            UsuarioBibliotecaViewModel modelo)
        {
            if (id != modelo.UsuarioBibliotecaId)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            var resultado = await _usuarioServicio
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
            var usuario = await _usuarioServicio
                .ObtenerPorIdAsync(id);

            if (usuario is null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarEliminacion(
            int usuarioBibliotecaId)
        {
            var resultado = await _usuarioServicio
                .EliminarAsync(usuarioBibliotecaId);

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