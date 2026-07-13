using BookCore.Helpers;
using BookCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookCore.Controllers
{
    [Authorize(Roles = "Usuario")]
    public class FavoritosController : Controller
    {
        private readonly IFavoritoServicio
            _favoritoServicio;

        public FavoritosController(
            IFavoritoServicio favoritoServicio)
        {
            _favoritoServicio =
                favoritoServicio;
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

            var favoritos = await _favoritoServicio
                .ObtenerPorUsuarioAsync(
                    usuarioId.Value);

            return View(favoritos);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Agregar(
            int libroId,
            string? returnUrl)
        {
            int? usuarioId =
                ObtenerUsuarioBibliotecaId();

            if (!usuarioId.HasValue)
            {
                return Forbid();
            }

            var resultado = await _favoritoServicio
                .AgregarAsync(
                    usuarioId.Value,
                    libroId);

            TempData[
                resultado.Exitoso
                    ? "MensajeExito"
                    : "MensajeError"
            ] = resultado.Mensaje;

            return Redirigir(returnUrl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(
            int libroId,
            string? returnUrl)
        {
            int? usuarioId =
                ObtenerUsuarioBibliotecaId();

            if (!usuarioId.HasValue)
            {
                return Forbid();
            }

            var resultado = await _favoritoServicio
                .EliminarAsync(
                    usuarioId.Value,
                    libroId);

            TempData[
                resultado.Exitoso
                    ? "MensajeExito"
                    : "MensajeError"
            ] = resultado.Mensaje;

            return Redirigir(returnUrl);
        }

        private IActionResult Redirigir(
            string? returnUrl)
        {
            if (!string.IsNullOrWhiteSpace(returnUrl) &&
                Url.IsLocalUrl(returnUrl))
            {
                return LocalRedirect(returnUrl);
            }

            return RedirectToAction(nameof(Index));
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