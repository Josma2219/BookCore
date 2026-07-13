using BookCore.Services;
using BookCore.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookCore.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class CuentasUsuariosController : Controller
    {
        private readonly ICuentaUsuarioServicio
            _cuentaServicio;

        public CuentasUsuariosController(
            ICuentaUsuarioServicio cuentaServicio)
        {
            _cuentaServicio = cuentaServicio;
        }

        [HttpGet]
        public async Task<IActionResult> Configurar(
            int usuarioBibliotecaId)
        {
            var modelo = await _cuentaServicio
                .PrepararAsync(usuarioBibliotecaId);

            if (modelo is null)
            {
                return NotFound();
            }

            return View(modelo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Configurar(
            CuentaUsuarioFormularioViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                var modeloOriginal = await _cuentaServicio
                    .PrepararAsync(
                        modelo.UsuarioBibliotecaId);

                modelo.UsuarioNombreCompleto =
                    modeloOriginal?
                        .UsuarioNombreCompleto
                    ?? string.Empty;

                modelo.TieneCuenta =
                    modeloOriginal?.TieneCuenta
                    ?? false;

                return View(modelo);
            }

            var resultado = await _cuentaServicio
                .GuardarAsync(modelo);

            if (!resultado.Exitoso)
            {
                ModelState.AddModelError(
                    string.Empty,
                    resultado.Mensaje);

                var modeloOriginal = await _cuentaServicio
                    .PrepararAsync(
                        modelo.UsuarioBibliotecaId);

                modelo.UsuarioNombreCompleto =
                    modeloOriginal?
                        .UsuarioNombreCompleto
                    ?? string.Empty;

                modelo.TieneCuenta =
                    modeloOriginal?.TieneCuenta
                    ?? false;

                return View(modelo);
            }

            TempData["MensajeExito"] =
                resultado.Mensaje;

            return RedirectToAction(
                "Detalle",
                "UsuariosBiblioteca",
                new
                {
                    id = modelo.UsuarioBibliotecaId
                });
        }
    }
}