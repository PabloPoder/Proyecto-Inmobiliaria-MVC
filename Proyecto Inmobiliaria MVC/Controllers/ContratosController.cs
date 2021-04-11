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
    public class ContratosController : Controller
    {
        protected readonly IConfiguration configuration;
        RepositorioContrato repositorioContrato;

        public ContratosController(IConfiguration configuration)
        {
            this.repositorioContrato = new RepositorioContrato(configuration);
            this.configuration = configuration;
        }

        // GET: ContratosController
        public ActionResult Index()
        {
            try
            {
                var lista = repositorioContrato.ObtenerTodos();
                ViewData[nameof(Contrato)] = lista;
                return View(lista);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        // GET: ContratosController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ContratosController/Create
        public ActionResult Create()
        { 
            return View();
        }

        // POST: ContratosController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Contrato contrato)
        {
            try
            {
                repositorioContrato.Alta(contrato);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ContratosController/Edit/5
        public ActionResult Edit(int id)
        {
            var contrato = repositorioContrato.ObtenerPorId(id);
            return View(contrato);
        }

        // POST: ContratosController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Contrato contrato)
        {
            try
            {
                contrato.Id = id;
                repositorioContrato.Modificacion(contrato);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ContratosController/Delete/5
        public ActionResult Delete(int id)
        {
            repositorioContrato.Baja(id);
            return View();
        }

        // POST: ContratosController/Delete/5
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
