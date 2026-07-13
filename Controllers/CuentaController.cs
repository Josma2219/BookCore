using System.Security.Claims;
using BookCore.Helpers;
using BookCore.Services;
using BookCore.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookCore.Controllers
{
    public class CuentaController : Controller
    {
        private readonly IAccesoServicio
            _accesoServicio;

        public CuentaController(
            IAccesoServicio accesoServicio)
        {
            _accesoServicio = accesoServicio;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult IniciarSesion(
            string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                if (User.IsInRole("Administrador"))
                {
                    return RedirectToAction(
                        "Index",
                        "Panel");
                }

                if (User.IsInRole("Usuario"))
                {
                    return RedirectToAction(
                        "Index",
                        "MiCuenta");
                }

                return RedirectToAction(
                    "Index",
                    "Home");
            }

            return View(new InicioSesionViewModel
            {
                ReturnUrl = returnUrl
            });
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IniciarSesion(
            InicioSesionViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            var usuario = await _accesoServicio
                .ValidarCredencialesAsync(
                    modelo.Identificador,
                    modelo.Contrasena);

            if (usuario is null)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "El usuario o la contraseña no son correctos.");

                return View(modelo);
            }

            var declaraciones = new List<Claim>
            {
                new(
                    ClaimTypes.NameIdentifier,
                    usuario.IdCuenta.ToString()),

                new(
                    ClaimTypes.Name,
                    usuario.NombreUsuario),

                new(
                    ClaimTypes.Email,
                    usuario.Correo),

                new(
                    ClaimTypes.Role,
                    usuario.Rol)
            };

            if (usuario.UsuarioBibliotecaId.HasValue)
            {
                declaraciones.Add(
                    new Claim(
                        TiposClaim.UsuarioBibliotecaId,
                        usuario.UsuarioBibliotecaId
                            .Value
                            .ToString()));
            }

            var identidad = new ClaimsIdentity(
                declaraciones,
                CookieAuthenticationDefaults
                    .AuthenticationScheme);

            var principal =
                new ClaimsPrincipal(identidad);

            var propiedades =
                new AuthenticationProperties
                {
                    IsPersistent =
                        modelo.Recordarme,

                    ExpiresUtc =
                        modelo.Recordarme
                            ? DateTimeOffset.UtcNow
                                .AddDays(7)
                            : null
                };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults
                    .AuthenticationScheme,
                principal,
                propiedades);

            TempData["MensajeExito"] =
                $"Bienvenido, {usuario.NombreUsuario}.";

            if (!string.IsNullOrWhiteSpace(
                    modelo.ReturnUrl) &&
                Url.IsLocalUrl(modelo.ReturnUrl))
            {
                return LocalRedirect(
                    modelo.ReturnUrl);
            }

            if (usuario.Rol == "Administrador")
            {
                return RedirectToAction(
                    "Index",
                    "Panel");
            }

            return RedirectToAction(
                "Index",
                "MiCuenta");
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CerrarSesion()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults
                    .AuthenticationScheme);

            return RedirectToAction(
                "Index",
                "Home");
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult AccesoDenegado()
        {
            return View();
        }
    }
}