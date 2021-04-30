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
    public class ContratosController : Controller
    {
        protected readonly IConfiguration configuration;
        RepositorioContrato repositorioContrato;
        private readonly RepositorioInmueble repositorioInmueble;
        private readonly RepositorioInquilino repositorioInquilino;
        private readonly RepositorioPago repositorioPago;


        public ContratosController(IConfiguration configuration)
        {
            repositorioContrato = new RepositorioContrato(configuration);
            repositorioInmueble = new RepositorioInmueble(configuration);
            repositorioInquilino = new RepositorioInquilino(configuration);
            repositorioPago = new RepositorioPago(configuration);
            this.configuration = configuration;
        }

        // GET: ContratosController
        public ActionResult Index()
        {
            try
            {
                ViewBag.Error = TempData["Error"];
                var lista = repositorioContrato.ObtenerTodos();
                return View(lista);
            }
            catch (Exception ex)
            {

                TempData["Error"] = "Error al ingresar al menu de contratos.";
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult ObtenerVigentes()
        {
            try
            {
                var lista = repositorioContrato.ObtenerContratosVigentes();
                return View("Index", lista);
            }
            catch(SqlException ex)
            {
                TempData["Error"] = "Ocurrio un error al intentar obtener contrato vigentes.";
                var lista = repositorioContrato.ObtenerTodos();
                return View("Index", lista);
            }
        }

        public ActionResult ObtenerExpirados()
        {
            try
            {
                var lista = repositorioContrato.ObtenerContratosExpirados();
                return View("Index", lista);
            }
            catch (SqlException ex)
            {
                TempData["Error"] = "Ocurrio un error al intentar obtener contrato vigentes.";
                var lista = repositorioContrato.ObtenerTodos();
                return View("Index", lista);
            }
        }

        public ActionResult PorInmueble(int id)
        {
            try
            {
                var lista = repositorioContrato.ObtenerPorInmueble(id);
                ViewBag.Id = id;
                return View("Index", lista);

            }
            catch (Exception)
            {
                TempData["Error"] = "Ocurrio un error al intentar obtener contrato por inmuebles.";
                var lista = repositorioContrato.ObtenerTodos();
                return View("Index", lista);
            }
        }

        public ActionResult PorInquilino(int id)
        {
            try
            {
                var lista = repositorioContrato.ObtenerPorInquilino(id);
                ViewBag.Id = id;
                return View("Index", lista);
            }
            catch (Exception)
            {
                TempData["Error"] = "Ocurrio un error al intentar obtener contrato por inquilinos.";
                var lista = repositorioContrato.ObtenerTodos();
                return View("Index", lista);
            }
        }

        public ActionResult BuscarPorFechas(DateTime fechaDesde, DateTime fechaHasta)
        {
            try
            {
                var lista = repositorioContrato.ContratosPorFechas(fechaDesde, fechaHasta);
                return View("Index", lista);
            }
            catch (SqlException ex)
            {
                TempData["Error"] = "Ocurrio un error al intentar buscar por fechas.";
                var lista = repositorioContrato.ObtenerTodos();
                return View("Index", lista);
            }
        }

        // GET: ContratosController/Create
        public ActionResult Create()
        {
            ViewBag.Inquilinos = repositorioInquilino.ObtenerTodos();
            ViewBag.Inmuebles = repositorioInmueble.ObtenerTodos();
            return View();
        }

        // POST: ContratosController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Contrato contrato)
        {
            try
            {
                var i  = repositorioInmueble.ObtenerUnInmueblePorFechas(contrato.InmuebleId, contrato.FechaDesde, contrato.FechaHasta);

                if(i != null)
                {
                    repositorioContrato.Alta(contrato);
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Error = "El inmueble esta ocupado en esas fechas.";
                    ViewBag.Inquilinos = repositorioInquilino.ObtenerTodos();
                    ViewBag.Inmuebles = repositorioInmueble.ObtenerTodos();
                    return View();
                }
            }
            catch (SqlException ex)
            {
                ViewBag.Error = "Ocurrio un error al intentar crear un contrato.";
                return View(contrato);
            }
        }

        // GET: ContratosController/Edit/5
        public ActionResult Edit(int id)
        {
            var contrato = repositorioContrato.ObtenerPorId(id);
            ViewBag.Inquilinos = repositorioInquilino.ObtenerTodos();
            ViewBag.Inmuebles = repositorioInmueble.ObtenerTodos();
            return View(contrato);
        }

        // POST: ContratosController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Contrato contrato)
        {
            try
            {
                var i = repositorioInmueble.ObtenerUnInmueblePorFechas(contrato.InmuebleId, contrato.FechaDesde, contrato.FechaHasta);

                if(i != null) 
                {
                    contrato.Id = id;
                    repositorioContrato.Modificacion(contrato);
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Error = "El inmueble esta ocupado en esas fechas.";
                    ViewBag.Inquilinos = repositorioInquilino.ObtenerTodos();
                    ViewBag.Inmuebles = repositorioInmueble.ObtenerTodos();
                    return View("Edit", contrato);
                }
            }
            catch (SqlException ex)
            {
                TempData["Error"] = "Ocurrio un error al intentar editar un contrato.";
                var lista = repositorioContrato.ObtenerTodos();
                return View("Index", lista);
            }
        }

        // GET: ContratosController/Renovar/5
        public ActionResult Renovar(int id, DateTime fechaDesde)
        {
            var contrato = repositorioContrato.ObtenerPorId(id);
            contrato.FechaDesde = fechaDesde;
            ViewBag.Inquilinos = repositorioInquilino.ObtenerTodos();
            ViewBag.Inmuebles = repositorioInmueble.ObtenerTodos();
            return View(contrato);
        }

        // POST: ContratosController/Renovar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Renovar(int id, Contrato contrato)
        {
            try
            {
                var i = repositorioInmueble.ObtenerUnInmueblePorFechas(contrato.InmuebleId, contrato.FechaDesde, contrato.FechaHasta);

                if (i != null)
                {
                    contrato.Id = id;
                    repositorioContrato.Modificacion(contrato);
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Error = "El inmueble esta ocupado en esas fechas.";
                    ViewBag.Inquilinos = repositorioInquilino.ObtenerTodos();
                    ViewBag.Inmuebles = repositorioInmueble.ObtenerTodos();
                    return View("Renovar", contrato);
                }
            }
            catch (SqlException ex)
            {
                TempData["Error"] = "Ocurrio un error al intentar editar un contrato.";
                var lista = repositorioContrato.ObtenerTodos();
                return View("Index", lista);
            }
        }


        // GET: ContratosController/Delete/5
        [Authorize(Policy = "Administrador")]
        public ActionResult Delete(int id)
        {
            try
            {
                repositorioContrato.Baja(id);
                return RedirectToAction(nameof(Index));
            }
            catch (SqlException ex)
            {
                TempData["Error"] = "Ocurrio un error al intentar borrar un usuario. ";
                var lista = repositorioContrato.ObtenerTodos();
                return View("Index", lista);
            }
        }

        // POST: ContratosController/Delete/5
        [Authorize(Policy = "Administrador")]
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
