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
            // Esta consulta confirma que la app puede leer la tabla Libro.
            ViewBag.TotalLibros = await _contexto
                .Set<Libro>()
                .CountAsync();

            // Este contador nos confirma que también podemos leer las categorías.
            ViewBag.TotalCategorias = await _contexto
                .Set<Categoria>()
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