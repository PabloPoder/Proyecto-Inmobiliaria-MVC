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
    public class InquilinosController : Controller
    {
        protected readonly IConfiguration configuration;
        RepositorioInquilino repositorioInquilino;

        public InquilinosController(IConfiguration configuration)
        {
            this.repositorioInquilino = new RepositorioInquilino(configuration);
            this.configuration = configuration;
        }

        // GET: InquilinoController
        public ActionResult Index()
        {
            try
            {
                ViewBag.Error = TempData["Error"];
                var lista = repositorioInquilino.ObtenerTodos();
                return View(lista);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Ocurrio un error al intentar ingresar al menu de inquilinos.";
                var lista = repositorioInquilino.ObtenerTodos();
                return View("Index", "Home");
            }
        }

        // GET: InquilinoController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: InquilinoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Inquilino inquilino)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    repositorioInquilino.Alta(inquilino);
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewBag.Inquilinos = repositorioInquilino.ObtenerTodos();
                    return View(inquilino);
                }
            }
            catch (SqlException ex) 
            {
                TempData["Error"] = "Ocurrio un error al intentar crear un inquilino.";
                var lista = repositorioInquilino.ObtenerTodos();
                return View("Index", lista);
            }
        }

        // GET: InquilinoController/Edit/5
        public ActionResult Edit(int id)
        {
            var inquilino = repositorioInquilino.ObtenerPorId(id);
            ViewBag.Inquilino = repositorioInquilino.ObtenerTodos();
            return View(inquilino);
        }

        // POST: InquilinoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Inquilino inquilino)
        {
            try
            {
                inquilino.Id = id;
                repositorioInquilino.Modificacion(inquilino);

                return RedirectToAction(nameof(Index));
            }
            catch (SqlException ex)
            {
                TempData["Error"] = "Ocurrio un error al intentar editar un inquilino.";
                var lista = repositorioInquilino.ObtenerTodos();
                return View("Index", lista);
            }
        }

        // GET: InquilinoController/Delete/5
        [Authorize(Policy = "Administrador")]
        public ActionResult Delete(int id)
        {
            try
            {
                repositorioInquilino.Baja(id);
                return RedirectToAction(nameof(Index));
            }
            catch (SqlException ex)
            {
                TempData["Error"] = "Ocurrio un error al intentar borrar un inquilino.";
                var lista = repositorioInquilino.ObtenerTodos();
                return View("Index", lista);
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
                TempData["Error"] = "Ocurrio un error al intentar borrar un inquilino.";
                var lista = repositorioInquilino.ObtenerTodos();
                return View("Index", lista);
            }
        }
    }
}
