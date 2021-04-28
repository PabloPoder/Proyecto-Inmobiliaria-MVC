using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Proyecto_Inmobiliaria_MVC.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Proyecto_Inmobiliaria_MVC.Controllers
{
    [Authorize]
    public class InmueblesController : Controller
    {
        protected readonly IConfiguration configuration;
        RepositorioInmueble repositorioInmueble;
        private readonly RepositorioPropietario repositorioPropietario;


        public InmueblesController(IConfiguration configuration)
        {
            this.repositorioInmueble = new RepositorioInmueble(configuration);
            this.repositorioPropietario = new RepositorioPropietario(configuration);
            this.configuration = configuration;
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
                    repositorioInmueble.Alta(inmueble);
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
                inmueble.Id = id;
                repositorioInmueble.Modificacion(inmueble);
                return RedirectToAction(nameof(Index));
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

    }
}

