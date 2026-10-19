using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoluntariadoAmbiental.Models;

namespace VoluntariadoAmbiental.Controllers
{
    public class CuentaController : Controller
    {
        private readonly Conexion conexion;

        public CuentaController()
        {
            conexion = new Conexion();
        }

        // =========================================================================
        // LOGIN
        // =========================================================================

        [HttpGet]
        public IActionResult Login(string? returnUrl)
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Voluntariado");
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            var usuario = conexion.ValidarUsuario(modelo.sEmail, modelo.sPassword);
            if (usuario == null)
            {
                ModelState.AddModelError("", "Credenciales incorrectas. Verifique correo o contraseña.");
                return View(modelo);
            }

            await IniciarSesionClaims(usuario, modelo.Recordarme);

            TempData["Exito"] = $"¡Bienvenido de nuevo, {usuario.sNombreCompleto}! Has ingresado con el rol de {usuario.sRol}.";

            if (!string.IsNullOrEmpty(modelo.ReturnUrl) && Url.IsLocalUrl(modelo.ReturnUrl))
            {
                return Redirect(modelo.ReturnUrl);
            }

            // Redirección según rol
            if (usuario.sRol == "Organizacion" || usuario.sRol == "Admin")
            {
                return RedirectToAction("Gestion", "Voluntariado");
            }
            else
            {
                return RedirectToAction("Index", "Voluntariado");
            }
        }

        // =========================================================================
        // REGISTRO
        // =========================================================================

        [HttpGet]
        public IActionResult Registro()
        {
            return View(new RegistroViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registro(RegistroViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            string resultado = conexion.RegistrarUsuario(modelo);
            if (resultado != "OK")
            {
                ModelState.AddModelError("", resultado);
                return View(modelo);
            }

            // Iniciar sesión después del registro
            Usuario? usuario = conexion.ValidarUsuario(modelo.sEmail, modelo.sPassword);
            if (usuario != null)
            {
                await IniciarSesionClaims(usuario, true);
                TempData["Exito"] = "¡Cuenta creada exitosamente! Bienvenido a la plataforma EcoCokie.";
                return RedirectToAction("Index", "Voluntariado");
            }

            TempData["Exito"] = "¡Cuenta creada exitosamente! Ya puedes iniciar sesión.";
            return RedirectToAction("Login");
        }

        // =========================================================================
        // LOGOUT
        // =========================================================================

        [HttpPost]
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["Exito"] = "Has cerrado sesión correctamente.";
            return RedirectToAction("Index", "Voluntariado");
        }

        // =========================================================================
        // PERFIL & ACCESO DENEGADO
        // =========================================================================

        [Authorize]
        [HttpGet]
        public IActionResult Perfil()
        {
            string? idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(idClaim, out int idUsuario))
            {
                Usuario? usuario = conexion.ObtenerUsuarioPorId(idUsuario);
                if (usuario != null)
                {
                    ViewBag.MisAsistencias = conexion.getAsistenciasPorUsuario(idUsuario);
                    return View(usuario);
                }
            }

            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult AccesoDenegado()
        {
            return View();
        }

        // =========================================================================
        // MÉTODO AUXILIAR PARA GENERAR CLAIMS Y COOKIE
        // =========================================================================

        private async Task IniciarSesionClaims(Usuario usuario, bool persistente)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.nIdUsuario.ToString()),
                new Claim(ClaimTypes.Name, usuario.sNombreCompleto),
                new Claim(ClaimTypes.Email, usuario.sEmail),
                new Claim(ClaimTypes.Role, usuario.sRol),
                new Claim("Carnet", usuario.sCarnet ?? ""),
                new Claim("Institucion", usuario.sInstitucion ?? ""),
                new Claim("Telefono", usuario.sTelefono ?? "")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = persistente,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
        }
    }
}
