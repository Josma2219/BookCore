using BookCore.Helpers;
using BookCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookCore.Controllers
{
    [Authorize(Roles = "Usuario")]
    public class MiCuentaController : Controller
    {
        private readonly IMiCuentaServicio
            _miCuentaServicio;

        public MiCuentaController(
            IMiCuentaServicio miCuentaServicio)
        {
            _miCuentaServicio =
                miCuentaServicio;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            int? usuarioId =
                ObtenerUsuarioBibliotecaId();

            if (!usuarioId.HasValue)
            {
                return Forbid();
            }

            var modelo = await _miCuentaServicio
                .ObtenerResumenAsync(
                    usuarioId.Value);

            if (modelo is null)
            {
                return NotFound();
            }

            return View(modelo);
        }

        [HttpGet]
        public async Task<IActionResult> Historial()
        {
            int? usuarioId =
                ObtenerUsuarioBibliotecaId();

            if (!usuarioId.HasValue)
            {
                return Forbid();
            }

            var modelo = await _miCuentaServicio
                .ObtenerHistorialAsync(
                    usuarioId.Value);

            if (modelo is null)
            {
                return NotFound();
            }

            return View(modelo);
        }

        private int? ObtenerUsuarioBibliotecaId()
        {
            string? valor = User.FindFirst(
                TiposClaim.UsuarioBibliotecaId)
                ?.Value;

            if (int.TryParse(valor, out int usuarioId))
            {
                return usuarioId;
            }

            return null;
        }
    }
}