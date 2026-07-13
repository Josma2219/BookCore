using BookCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookCore.Controllers
{
    [AllowAnonymous]
    public class CatalogoController : Controller
    {
        private readonly ICatalogoServicio _catalogoServicio;

        public CatalogoController(
            ICatalogoServicio catalogoServicio)
        {
            _catalogoServicio = catalogoServicio;
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

            return View(libro);
        }
    }
}