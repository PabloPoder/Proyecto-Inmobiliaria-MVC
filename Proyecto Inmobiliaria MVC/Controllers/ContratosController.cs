﻿using Microsoft.AspNetCore.Http;
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
    public class ContratosController : Controller
    {
        protected readonly IConfiguration configuration;
        RepositorioContrato repositorioContrato;
        private readonly RepositorioInmueble repositorioInmueble;
        private readonly RepositorioInquilino repositorioInquilino;


        public ContratosController(IConfiguration configuration)
        {
            repositorioContrato = new RepositorioContrato(configuration);
            repositorioInmueble = new RepositorioInmueble(configuration);
            repositorioInquilino = new RepositorioInquilino(configuration);
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
                if (ModelState.IsValid)
                {
                    repositorioContrato.Alta(contrato);
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewBag.Inquilinos = repositorioContrato.ObtenerTodos();
                    return View(contrato);
                }

            }
            catch (SqlException ex)
            {
                ViewBag.Error = "Ocurrio un error " + ex.Message;
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
                contrato.Id = id;
                repositorioContrato.Modificacion(contrato);

                return RedirectToAction(nameof(Index));
            }
            catch (SqlException ex)
            {
                TempData["Error"] = "Ocurrio un error " + ex.ToString();
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: ContratosController/Delete/5
        public ActionResult Delete(int id)
        {
            try
            {
                repositorioContrato.Baja(id);
                return RedirectToAction(nameof(Index));
            }
            catch (SqlException ex)
            {
                TempData["Error"] = "Ocurrio un error " + ex.ToString();
                return RedirectToAction(nameof(Index));
            }
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
