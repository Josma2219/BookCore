using BookCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookCore.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class PanelController : Controller
    {
        private readonly IPanelAdministrativoServicio
            _panelServicio;

        public PanelController(
            IPanelAdministrativoServicio panelServicio)
        {
            _panelServicio = panelServicio;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var modelo = await _panelServicio
                .ObtenerPanelAsync();

            return View(modelo);
        }
    }
}