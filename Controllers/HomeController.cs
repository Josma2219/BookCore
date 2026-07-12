using System.Diagnostics;
using BookCore.Data;
using BookCore.Models;
using BookCore.Models.Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookCore.Controllers
{
    public class HomeController : Controller
    {
        private readonly BookCoreContexto _contexto;

        public HomeController(BookCoreContexto contexto)
        {
            _contexto = contexto;
        }

        public async Task<IActionResult> Index()
        {
            // Estos contadores luego serán parte del dashboard.
            ViewBag.TotalLibros = await _contexto
                .Set<Libro>()
                .CountAsync();

            ViewBag.TotalCategorias = await _contexto
                .Set<Categoria>()
                .CountAsync();

            ViewBag.TotalAutores = await _contexto
                .Set<Autor>()
                .CountAsync();

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(
            Duration = 0,
            Location = ResponseCacheLocation.None,
            NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id
                    ?? HttpContext.TraceIdentifier
            });
        }
    }
}