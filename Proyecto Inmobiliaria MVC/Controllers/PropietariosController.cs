using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Proyecto_Inmobiliaria_MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proyecto_Inmobiliaria_MVC.Controllers
{
    public class PropietariosController : Controller
    {
        protected readonly IConfiguration configuration;
        RepositorioPropietario repositorioPropietario;

        public PropietariosController(RepositorioPropietario repositorioPropietario, IConfiguration configuration)
        {
            this.repositorioPropietario = new RepositorioPropietario(configuration);
            this.configuration = configuration;
        }

        // GET: PersonasController
        public ActionResult Index()
        {
            try
            {
                var lista = repositorioPropietario.ObtenerTodos();
                return View(lista);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        // GET: PersonasController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: PersonasController/Create
        [HttpPost]
        public ActionResult Crear(Propietario propietario) 
        {
            repositorioPropietario.Alta(propietario);
            return RedirectToAction(nameof(Index));
        }

        // POST: PersonasController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: PersonasController/Edit/5
        public ActionResult Edit(Propietario propietario)
        {
            repositorioPropietario.Modificacion(propietario);
            return RedirectToAction(nameof(Index));
        }

        // POST: PersonasController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: PersonasController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: PersonasController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
