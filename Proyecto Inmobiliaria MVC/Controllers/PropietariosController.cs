using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
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
    public class PropietariosController : Controller
    {
        protected readonly IConfiguration configuration;
        RepositorioPropietario repositorioPropietario;

        public PropietariosController(IConfiguration configuration)
        {
            this.repositorioPropietario = new RepositorioPropietario(configuration);
            this.configuration = configuration;
        }

        // GET: PersonasController
        public ActionResult Index()
        {
            try
            {
                ViewBag.Error = TempData["Error"];
                var lista = repositorioPropietario.ObtenerTodos();
                return View(lista);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Ocurrio un error al intentar ingresar al menu de propietarios.";
                return RedirectToAction("Index", "Home");
            }
        }


        // GET: PersonasController/Create
        public ActionResult Create() 
        {
            return View();
        }

        // POST: PersonasController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Propietario propietario)
        {
            try
            {
                propietario.Clave = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                             password: propietario.Clave,
                             salt: System.Text.Encoding.ASCII.GetBytes(configuration["Salt"]),
                             prf: KeyDerivationPrf.HMACSHA1,
                             iterationCount: 1000,
                             numBytesRequested: 256 / 8));

                repositorioPropietario.Alta(propietario);
                return RedirectToAction(nameof(Index));
            }
            catch (SqlException ex)
            {
                TempData["Error"] = "Ocurrio un error al intentar crear un propietario.";
                var lista = repositorioPropietario.ObtenerTodos();
                return View("Index", lista);
            }
        }

        // GET: PersonasController/Edit/5
        public ActionResult Edit(int id)
        {
            var contrato = repositorioPropietario.ObtenerPorId(id);
            return View(contrato);
        }

        // POST: PersonasController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Propietario propietario)
        {
            try
            {
                propietario.Id = id;

                propietario.Clave = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                             password: propietario.Clave,
                             salt: System.Text.Encoding.ASCII.GetBytes(configuration["Salt"]),
                             prf: KeyDerivationPrf.HMACSHA1,
                             iterationCount: 1000,
                             numBytesRequested: 256 / 8));

                repositorioPropietario.Modificacion(propietario);

                return RedirectToAction(nameof(Index));
            }
            catch (SqlException ex)
            {
                TempData["Error"] = "Ocurrio un error al intentar editar un propietario.";
                var lista = repositorioPropietario.ObtenerTodos();
                return View("Index", lista);
            }
        }

        // GET: PersonasController/Delete/5
        [Authorize(Policy = "Administrador")]
        public ActionResult Delete(int id)
        {
            try
            {
                repositorioPropietario.Baja(id);
                return RedirectToAction(nameof(Index));
            }
            catch (SqlException ex)
            {
                TempData["Error"] = "Ocurrio un error al intentar borrar un propietario.";
                var lista = repositorioPropietario.ObtenerTodos();
                return View("Index", lista);
            }
        }

        // POST: PersonasController/Delete/5
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
