using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Proyecto_Inmobiliaria_MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proyecto_Inmobiliaria_MVC.Controllers
{
    public class PropietariosController : Controller
    {

        private RepositorioPropietario repositorioPropietario;


        // GET: PersonasController
        public ActionResult Index()
        {
            repositorioPropietario = new RepositorioPropietario();

            var lista = repositorioPropietario.ObtenerTodos();
            ViewData[nameof(Propietario)] = lista;

            return View();
        }

        // GET: PersonasController/Details/5
        public ActionResult Detalles(int id)
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
        public ActionResult Crear(IFormCollection collection)
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
        public ActionResult Editar(int id)
        {
            return View();
        }

        // POST: PersonasController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(int id, IFormCollection collection)
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
