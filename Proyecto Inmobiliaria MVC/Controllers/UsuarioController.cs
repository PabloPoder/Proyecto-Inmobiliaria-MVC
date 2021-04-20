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
    public class UsuarioController : Controller
    {
        protected readonly IConfiguration configuration;
        RepositorioUsuario repositorioUsuario;

        public UsuarioController(IConfiguration configuration)
        {
            this.repositorioUsuario = new RepositorioUsuario(configuration);
            this.configuration = configuration;
        }

        // GET: UsuarioController
        public ActionResult Index()
        {
            try
            {
                var lista = repositorioUsuario.ObtenerTodos();
                ViewData[nameof(Usuario)] = lista;
                return View(lista);
            }
            catch (SqlException ex)
            {
                TempData["Error"] = "Ocurrio un error " + ex.ToString();
                return RedirectToAction(nameof(Index));
            }
            
        }

        // GET: UsuarioController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: UsuarioController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UsuarioController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Usuario usuario)
        {
            try
            {
                repositorioUsuario.Alta(usuario);
                return RedirectToAction(nameof(Index));
            }
            catch (SqlException ex)
            {
                TempData["Error"] = "Ocurrio un error " + ex.ToString();
                return RedirectToAction(nameof(Index));
            }

        }

        // GET: UsuarioController/Edit/5
        public ActionResult Edit(int id)
        {
            var usuario = repositorioUsuario.ObtenerPorId(id);
            return View(usuario);
        }

        // POST: UsuarioController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Usuario usuario)
        {
            try
            {
                usuario.Id = id;
                repositorioUsuario.Modificacion(usuario);
                return RedirectToAction(nameof(Index));
            }
            catch (SqlException ex)
            {
                TempData["Error"] = "Ocurrio un error " + ex.ToString();
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: UsuarioController/Delete/5
        public ActionResult Delete(int id)
        {
            repositorioUsuario.Baja(id);
            return View();
        }

        // POST: UsuarioController/Delete/5
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
