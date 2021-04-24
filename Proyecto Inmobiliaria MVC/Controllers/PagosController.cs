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
    public class PagosController : Controller
    {
        RepositorioPago repositorioPago;
        RepositorioContrato repositorioContrato;
        private IConfiguration configuration;

        public PagosController(IConfiguration configuration)
        {
            repositorioPago = new RepositorioPago(configuration);
            repositorioContrato = new RepositorioContrato(configuration);
            this.configuration = configuration;
        }

        // GET: PagosController
        public ActionResult Index()
        {

            return View();
        }

        public ActionResult ObtenerPagos(int id)
        {
            var lista = repositorioPago.ObtenerTodos(id);
            ViewBag.ContratoId = id;
            return View("Index", lista);
        }

        // GET: PagosController/Create
        public ActionResult Create(int id)
        {
            ViewBag.Contrato = repositorioContrato.ObtenerPorId(id);
            return View();
        }

        // POST: PagosController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(int id, Pago pago)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    pago.ContratoId = id;
                    repositorioPago.Alta(pago);

                    var lista = repositorioPago.ObtenerTodos(id);
                    return View("Index", lista);
                }
                else
                {
                    
                    return View();
                }

            }
            catch (SqlException ex)
            {
                TempData["Error"] = "Ocurrio un error " + ex.ToString();
                return RedirectToAction(nameof(Index));
            }
        }

    }
}
