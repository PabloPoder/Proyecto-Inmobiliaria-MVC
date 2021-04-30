using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Hosting;
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
        private readonly IWebHostEnvironment environment;
        RepositorioUsuario repositorioUsuario;

        public UsuariosController(IConfiguration configuration, IWebHostEnvironment environment)
        {
            this.repositorioUsuario = new RepositorioUsuario(configuration);
            this.configuration = configuration;
            this.environment = environment;
        }

        // GET: UsuarioController
        public ActionResult Index()
        {
            try
            {
                ViewData["Error"] = TempData["Error"];
                var lista = repositorioUsuario.ObtenerTodos();
                return View(lista);
            }
            catch (SqlException ex)
            {
                TempData["Error"] = "Ocurrio un error al intentar ingresar al menu de usuarios.";
                return RedirectToAction("Index", "Home");
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

                    //TempData.Remove("returnUrl");
                    return Redirect(returnUrl);
                }
                //TempData["returnUrl"] = returnUrl;
                return View();
            }
            catch(Exception ex)
            {
                TempData["Error"] = "Ocurrio un error al intentar logearse.";
                var lista = repositorioUsuario.ObtenerTodos();
                return View("Index", lista);
            }
        }

        // GET: Usuarios/Perfil/5
        public ActionResult Perfil()
        {
            try
            {
                var usuario = repositorioUsuario.ObtenerPorEmail(User.Identity.Name);
                ViewBag.Roles = Usuario.ObtenerRoles();
                return View("Edit", usuario);
            }
            catch (Exception)
            {
                TempData["Error"] = "Ocurrio un error al intentar editar el usuario.";
                var lista = repositorioUsuario.ObtenerTodos();
                return View("Index", lista);
            }
            
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
            var usuarioActual = repositorioUsuario.ObtenerPorEmail(usuario.Email);
            if(usuarioActual.Email == null)
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

                        if (usuario.AvatarFile != null && usuario.Id > 0)
                        {
                            string wwwPath = environment.WebRootPath;
                            string path = Path.Combine(wwwPath, "Uploads");
                            if (!Directory.Exists(path))
                            {
                                Directory.CreateDirectory(path);
                            }
                            //Path.GetFileName(u.AvatarFile.FileName);//este nombre se puede repetir
                            string fileName = "avatar_" + usuario.Id + Path.GetExtension(usuario.AvatarFile.FileName);
                            string pathCompleto = Path.Combine(path, fileName);
                            usuario.Avatar = Path.Combine("/Uploads", fileName);
                            using (FileStream stream = new FileStream(pathCompleto, FileMode.Create))
                            {
                                usuario.AvatarFile.CopyTo(stream);
                            }
                            repositorioUsuario.Modificacion(usuario);
                        }

                        return RedirectToAction(nameof(Index));

                    }
                    catch (Exception ex)
                    {
                        TempData["Error"] = "Ocurrio un error al intentar crear un usuario.";
                        var lista = repositorioUsuario.ObtenerTodos();
                        return View("Index", lista);
                    }
                }
                else
                {
                    TempData["Error"] = "Ocurrio un error al intentar editar el usuario.";
                    return View();
                }
            }
            else
            {
                TempData["Error"] = "Ocurrio un error al intentar crear el usuario.";
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
            var usuarioActual = repositorioUsuario.ObtenerPorEmail(User.Identity.Name);

            if (User.IsInRole("Administrador") || User.IsInRole("SuperAdministrador") || usuarioActual.Id == id)
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

                    if (usuario.AvatarFile != null && usuario.Id > 0)
                    {
                        string wwwPath = environment.WebRootPath;
                        string path = Path.Combine(wwwPath, "Uploads");
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        //Path.GetFileName(u.AvatarFile.FileName);//este nombre se puede repetir
                        string fileName = "avatar_" + usuario.Id + Path.GetExtension(usuario.AvatarFile.FileName);
                        string pathCompleto = Path.Combine(path, fileName);
                        usuario.Avatar = Path.Combine("/Uploads", fileName);
                        using (FileStream stream = new FileStream(pathCompleto, FileMode.Create))
                        {
                            usuario.AvatarFile.CopyTo(stream);
                        }
                        repositorioUsuario.Modificacion(usuario);
                    }
                    else
                    {
                        var usuarioOriginal = repositorioUsuario.ObtenerPorId(usuario.Id);
                        usuario.Avatar = usuarioOriginal.Avatar;
                        repositorioUsuario.Modificacion(usuario);
                    }

                    return RedirectToAction(nameof(Index));
                }
                catch (SqlException ex)
                {
                    TempData["Error"] = "Ocurrio un error al intentar editar el usuario.";
                    var lista = repositorioUsuario.ObtenerTodos();
                    return View("Index", lista);
                }
            }
            else
            {
                TempData["Error"] = "Ocurrio un error al intentar editar el usuario.";
                var lista = repositorioUsuario.ObtenerTodos();
                return View("Index", lista);
            }
        }

        [Authorize(Policy = "Administrador")]
        // GET: UsuarioController/Delete/5
        public ActionResult Delete(int id)
        {
            try
            {
                repositorioUsuario.Baja(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {

                TempData["Error"] = "Ocurrio un error al intentar borrar el usuario.";
                var lista = repositorioUsuario.ObtenerTodos();
                return View("Index", lista);
            }
            
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
