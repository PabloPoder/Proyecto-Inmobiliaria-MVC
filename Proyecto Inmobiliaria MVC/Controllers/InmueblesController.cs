using Microsoft.AspNetCore.Authorization;
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
using System.Threading.Tasks;

namespace Proyecto_Inmobiliaria_MVC.Controllers
{
    [Authorize]
    public class InmueblesController : Controller
    {
        protected readonly IConfiguration configuration;
        RepositorioInmueble repositorioInmueble;
        private readonly IWebHostEnvironment environment;
        private readonly RepositorioPropietario repositorioPropietario;


        public InmueblesController(IConfiguration configuration, IWebHostEnvironment environment)
        {
            this.repositorioInmueble = new RepositorioInmueble(configuration);
            this.repositorioPropietario = new RepositorioPropietario(configuration);
            this.configuration = configuration;
            this.environment = environment;
        }

        // GET: InquilinoController
        public ActionResult Index()
        {
            try
            {
                ViewBag.Error = TempData["Error"];
                var lista = repositorioInmueble.ObtenerTodos();
                return View(lista);
            }
            catch (SqlException ex)
            {
                TempData["Error"] = "Ocurrio un error " + ex.ToString();
                return RedirectToAction(nameof(Index));
            }
        }

        public ActionResult PorPropietario(int id)
        {
            var lista = repositorioInmueble.ObtenerPorPropietario(id);
            ViewBag.Id = id;

            return View("Index", lista);
        }

        // GET: Inmueble/Create
        public ActionResult Create()
        {
            ViewBag.Propietarios = repositorioPropietario.ObtenerTodos();
            return View();
        }

        // POST: InquilinoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Inmueble inmueble)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    int res = repositorioInmueble.Alta(inmueble);

                    if (inmueble.FotoFile != null && inmueble.Id > 0)
                    {
                        string wwwPath = environment.WebRootPath;
                        string path = Path.Combine(wwwPath, "Uploads");
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        //Path.GetFileName(u.AvatarFile.FileName);//este nombre se puede repetir
                        string fileName = "foto_" + inmueble.Id + Path.GetExtension(inmueble.FotoFile.FileName);
                        string pathCompleto = Path.Combine(path, fileName);
                        inmueble.Foto = Path.Combine("/Uploads", fileName);
                        using (FileStream stream = new FileStream(pathCompleto, FileMode.Create))
                        {
                            inmueble.FotoFile.CopyTo(stream);
                        }
                        repositorioInmueble.Modificacion(inmueble);
                    }

                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewBag.Inmuebles = repositorioInmueble.ObtenerTodos();
                    return View(inmueble);
                }

            }
            catch (SqlException ex)
            {
                TempData["Error"] = "Ocurrio un error " + ex.ToString();
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: InquilinoController/Edit/5
        public ActionResult Edit(int id)
        {
            var inmueble = repositorioInmueble.ObtenerPorId(id);
            ViewBag.Propietarios = repositorioPropietario.ObtenerTodos();
            return View(inmueble);
        }

        // POST: InquilinoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Inmueble inmueble)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (inmueble.FotoFile != null && inmueble.Id > 0)
                    {
                        string wwwPath = environment.WebRootPath;
                        string path = Path.Combine(wwwPath, "Uploads");
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        //Path.GetFileName(u.AvatarFile.FileName);//este nombre se puede repetir
                        string fileName = "foto_" + inmueble.Id + Path.GetExtension(inmueble.FotoFile.FileName);
                        string pathCompleto = Path.Combine(path, fileName);
                        inmueble.Foto = Path.Combine("/Uploads", fileName);
                        using (FileStream stream = new FileStream(pathCompleto, FileMode.Create))
                        {
                            inmueble.FotoFile.CopyTo(stream);
                        }
                        repositorioInmueble.Modificacion(inmueble);
                    }
                    else
                    {
                        var usuarioOriginal = repositorioInmueble.ObtenerPorId(inmueble.Id);
                        inmueble.Foto = usuarioOriginal.Foto;
                        repositorioInmueble.Modificacion(inmueble);
                    }

                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewBag.Inmuebles = repositorioInmueble.ObtenerTodos();
                    return View(inmueble);
                }

            }
            catch (SqlException ex)
            {
                TempData["Error"] = "Ocurrio un error " + ex.ToString();
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: InquilinoController/Delete/5
        [Authorize(Policy = "Administrador")]
        public ActionResult Delete(int id)
        {
            try
            {
                repositorioInmueble.Baja(id);
                return RedirectToAction(nameof(Index));
            }
            catch (SqlException ex)
            {
                TempData["Error"] = "Ocurrio un error " + ex.ToString();
                var lista = repositorioInmueble.ObtenerTodos();
                return View("Index");
            }
        }

        // POST: InquilinoController/Delete/5
        [Authorize(Policy = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch (SqlException ex)
            {
                TempData["Error"] = "Ocurrio un error " + ex.ToString();
                return RedirectToAction(nameof(Index));
            }
        }

        /*
        public ActionResult BuscarPorFechas(DateTime fechaDesde, DateTime fechaHasta)
        {
            try
            {
                if (fechaDesde < fechaHasta)
                {
                    var lista = repositorioInmueble.ObtenerInmueblesPorFechas(fechaDesde, fechaHasta);
                    return View("Index", lista);
                }
                else
                {
                    TempData["Error"] = "Error al filtrar inmuebles, asegurese de ingresar bien las fechas.";
                    var lista = repositorioInmueble.ObtenerTodos();
                    return View("Index", lista);
                }
            }
            catch (SqlException ex)
            {
                TempData["Error"] = "Error al filtrar inmuebles.";
                var lista = repositorioInmueble.ObtenerTodos();
                return View("Index", lista);
            }
        }
        */

    }
}

