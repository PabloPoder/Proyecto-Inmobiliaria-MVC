using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Proyecto_Inmobiliaria_MVC.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Proyecto_Inmobiliaria_MVC.Controllers
{
    [Authorize]
    public class UsuariosController : Controller
    {
        protected readonly IConfiguration configuration;
        RepositorioUsuario repositorioUsuario;

        public UsuariosController(IConfiguration configuration)
        {
            this.repositorioUsuario = new RepositorioUsuario(configuration);
            this.configuration = configuration;
        }

        // GET: UsuarioController
        public ActionResult Index()
        {
            try
            {
                var lista = repositorioUsuario.ObtenerTodos();
                ViewData[nameof(Usuario)] = lista;
                return View(lista);
            }
            catch (SqlException ex)
            {
                TempData["Error"] = "Ocurrio un error " + ex.ToString();
                return RedirectToAction(nameof(Index));
            }
        }

        [Route("salir", Name = "logout")]
        // GET: Usuarios/Logout/
        public async Task<ActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        // GET: Usuario/Login/
        [AllowAnonymous]
        public ActionResult Login (string returnUrl)
        {
            TempData["returnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        // POST: Usuario/Login/
        public async Task<ActionResult> Login(LoginView login)
        {
            try
            {
                var returnUrl = String.IsNullOrEmpty(TempData["returnUrl"] as string) ? "/Home" : TempData["returnUrl"].ToString();
                if (ModelState.IsValid)
                {
                    string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                        password: login.Clave,
                        salt: System.Text.Encoding.ASCII.GetBytes(configuration["Salt"]),
                        prf: KeyDerivationPrf.HMACSHA1,
                        iterationCount: 1000,
                        numBytesRequested: 256 / 8));

                    var usuario = repositorioUsuario.ObtenerPorEmail(login.Usuario);
                    if(usuario == null || usuario.Clave != hashed)
                    {
                        ModelState.AddModelError("", "El email o la clave no son correctos");
                        /*TempData["returnUrl"] = returnUrl;*/
                        return View();
                    }

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, usuario.Email),
                        new Claim("FullName", usuario.Nombre + " " + usuario.Apellido),
                        new Claim(ClaimTypes.Role, usuario.RolNombre),
                    };

                    var claimsIdentity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity));

                    TempData.Remove("returnUrl");
                    return Redirect(returnUrl);
                }
                TempData["returnUrl"] = returnUrl;
                return View();
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }
        }

        // GET: Usuarios/Perfil/5
        public ActionResult Perfil()
        {
            var usuario = repositorioUsuario.ObtenerPorEmail(User.Identity.Name);
            ViewBag.Roles = Usuario.ObtenerRoles();
            return View("Edit", usuario);
        }


        // GET: UsuarioController/Create
        public ActionResult Create()
        {
            ViewBag.Roles = Usuario.ObtenerRoles();
            return View();
        }

        // POST: UsuarioController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Usuario usuario)
        {
            
            if (ModelState.IsValid)
            {
                try
                {
                    usuario.Clave = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                             password: usuario.Clave,
                             salt: System.Text.Encoding.ASCII.GetBytes(configuration["Salt"]),
                             prf: KeyDerivationPrf.HMACSHA1,
                             iterationCount: 1000,
                             numBytesRequested: 256 / 8));

                    usuario.Rol = 3;
                    int res = repositorioUsuario.Alta(usuario);

                    return RedirectToAction(nameof(Index));

                }
                catch (Exception ex)
                {
                    ViewBag.Roles = Usuario.ObtenerRoles();
                    return View();
                }
            }
            else
            {
                return View();
            }
           
        }

        // GET: UsuarioController/Edit/5
        public ActionResult Edit(int id)
        {
            var usuario = repositorioUsuario.ObtenerPorId(id);
            ViewBag.Roles = Usuario.ObtenerRoles();
            return View(usuario);
        }

        // POST: UsuarioController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Usuario usuario)
        {
            try
            {
                usuario.Id = id;

                usuario.Clave = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                            password: usuario.Clave,
                            salt: System.Text.Encoding.ASCII.GetBytes(configuration["Salt"]),
                            prf: KeyDerivationPrf.HMACSHA1,
                            iterationCount: 1000,
                            numBytesRequested: 256 / 8));

                repositorioUsuario.Modificacion(usuario);

                return RedirectToAction(nameof(Index));
            }
            catch (SqlException ex)
            {
                TempData["Error"] = "Ocurrio un error " + ex.ToString();
                return RedirectToAction(nameof(Index));
            }
        }

        [Authorize(Policy = "Administrador")]
        // GET: UsuarioController/Delete/5
        public ActionResult Delete(int id)
        {
            repositorioUsuario.Baja(id);
            return RedirectToAction(nameof(Index));
        }

        // POST: UsuarioController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrador")]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
