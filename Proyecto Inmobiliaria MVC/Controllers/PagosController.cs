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
            try
            {
                ViewBag.Error = TempData["Error"];
                return View();
            }
            catch (Exception)
            {

                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult ObtenerPagos(int id)
        {
            try
            {
                var lista = repositorioPago.ObtenerTodos(id);
                ViewBag.ContratoId = id;
                return View("Index", lista);
            }
            catch (Exception)
            {

                TempData["Error"] = "Ocurrio un error al intentar ingresar al menu de pagos.";
                return RedirectToAction("Index", "Home");
            }
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
                    var contrato = repositorioContrato.ObtenerPorId(id);

                    pago.Precio = contrato.Inmueble.Precio;

                    repositorioPago.Alta(pago);

                    var lista = repositorioPago.ObtenerTodos(id);
                    return View("Index", lista);
                }
                else
                {
                    TempData["Error"] = "Ocurrio un error al intentar crear un pago.";
                    var lista = repositorioPago.ObtenerTodos(id);
                    return View();
                }

            }
            catch (SqlException ex)
            {
                TempData["Error"] = "Ocurrio un error al intentar crear un pago.";
                var lista = repositorioPago.ObtenerTodos(id);
                return View("Index", lista);
            }
        }

        // GET: UsuarioController/Edit/5
        public ActionResult Edit(int id)
        {
            var pago = repositorioPago.ObtenerPorId(id);
            ViewBag.IdContrato = pago.ContratoId;
            return View(pago);
        }

        // POST: UsuarioController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Pago pago)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    pago.Id = id;

                    repositorioPago.Modificacion(pago);

                    var lista = repositorioPago.ObtenerTodos(pago.ContratoId);

                    return View("Index", lista);
                }
                else
                {
                    var lista = repositorioPago.ObtenerTodos(pago.ContratoId);
                    TempData["Error"] = "Ocurrio un error al modificar el Pago";
                    return View("Index", lista);
                }

            }
            catch (SqlException ex)
            {
                TempData["Error"] = "Ocurrio un error al intentar crear un pago.";
                return RedirectToAction("Index", "Contratos");

            }
        }

        // GET: InquilinoController/Delete/5
        [Authorize(Policy = "Administrador")]
        public ActionResult Delete(int id, int idContrato)
        {
            try
            {
                repositorioPago.Baja(id);
                return RedirectToAction("Index", "Contratos");
            }
            catch (SqlException ex)
            {
                TempData["Error"] = "Ocurrio un error al intentar borrar un pago.";
                var lista = repositorioPago.ObtenerTodos(idContrato);
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
                TempData["Error"] = "Ocurrio un error " + ex.ToString();
                return RedirectToAction(nameof(Index));
            }
        }

    }
}
