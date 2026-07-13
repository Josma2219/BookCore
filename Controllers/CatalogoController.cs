using BookCore.Helpers;
using BookCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookCore.Controllers
{
    [AllowAnonymous]
    public class CatalogoController : Controller
    {
        private readonly ICatalogoServicio
            _catalogoServicio;

        private readonly IFavoritoServicio
            _favoritoServicio;

        public CatalogoController(
            ICatalogoServicio catalogoServicio,
            IFavoritoServicio favoritoServicio)
        {
            _catalogoServicio =
                catalogoServicio;

            _favoritoServicio =
                favoritoServicio;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            string? busqueda,
            int? categoriaId)
        {
            var modelo = await _catalogoServicio
                .ObtenerIndiceAsync(
                    busqueda,
                    categoriaId);

            ViewBag.Favoritos =
                await ObtenerFavoritosUsuarioAsync();

            return View(modelo);
        }

        [HttpGet]
        public async Task<IActionResult> Detalle(int id)
        {
            var libro = await _catalogoServicio
                .ObtenerDetalleAsync(id);

            if (libro is null)
            {
                return NotFound();
            }

            var favoritos =
                await ObtenerFavoritosUsuarioAsync();

            ViewBag.EsFavorito =
                favoritos.Contains(id);

            return View(libro);
        }

        private async Task<HashSet<int>>
            ObtenerFavoritosUsuarioAsync()
        {
            if (!User.IsInRole("Usuario"))
            {
                return [];
            }

            string? valor = User.FindFirst(
                TiposClaim.UsuarioBibliotecaId)
                ?.Value;

            if (!int.TryParse(
                valor,
                out int usuarioId))
            {
                return [];
            }

            var ids = await _favoritoServicio
                .ObtenerIdsAsync(usuarioId);

            return ids.ToHashSet();
        }
    }
}