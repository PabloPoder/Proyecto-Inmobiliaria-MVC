using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Proyecto_Inmobiliaria_MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Proyecto_Inmobiliaria_MVC.Api
{

    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class ContratosController : ControllerBase
    {
        private readonly DataContext contexto;
        private readonly IConfiguration config;

        public ContratosController(DataContext contexto, IConfiguration config)
        {
            this.contexto = contexto;
            this.config = config;
        }

        // GET: api/<ContratosController>
        [HttpGet("ContratosPorInmueble/{id}")]
        public async Task<ActionResult> ContratosPorInmueble(int id)
        {
            try
            {
                var usuario = User.Identity.Name;

                return Ok(contexto.Contratos.Include(x => x.Inquilino)
                                            .Include(x => x.Inmueble)
                                            .ThenInclude(x => x.Propietario)
                                            .Where(x => x.Inmueble.Propietario.Email == usuario && x.Inmueble.Id == id));

            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // GET: api/<ContratosController>
        [HttpGet("TodosLosContratos")]
        public async Task<ActionResult> TodosLosContratos()
        {
            try
            {
                var usuario = User.Identity.Name;

                return Ok(contexto.Contratos.Include(x => x.Inquilino)
                                            .Include(x => x.Inmueble)
                                            .ThenInclude(x => x.Propietario)
                                            .Where(x => x.Inmueble.Propietario.Email == usuario));

            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // GET: api/<ContratosController>
        [HttpGet("ContratosVigentes")]
        public async Task<ActionResult> ContratosVigentes()
        {
            try
            {
                var usuario = User.Identity.Name;

                return Ok(contexto.Contratos.Include(x => x.Inquilino)
                                            .Include(x => x.Inmueble)
                                            .ThenInclude(x => x.Propietario)
                                            .Where(x => x.Inmueble.Propietario.Email == usuario && (x.FechaHasta > DateTime.Now && x.Estado == true)));

            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

        }

        // GET: api/<ContratosController>
        [HttpGet("ContratoVigentePorInmueble/{id}")]
        public async Task<ActionResult> ContratoVigentePorInmueble(int id)
        {
            try
            {
                var usuario = User.Identity.Name;

                return Ok(contexto.Contratos.Include(x => x.Inquilino)
                                            .Include(x => x.Inmueble)
                                            .ThenInclude(x => x.Propietario)
                                            .Where(x => x.Inmueble.Propietario.Email == usuario && (x.FechaHasta > DateTime.Now && x.Estado == true) && x.Estado == true)
                                            .Single());

            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

        }

        //ContratoDetallado
        // GET api/<ContratosController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult> Contrato(int id)
        {
            try
            {
                var usuario = User.Identity.Name;

                return Ok(contexto.Contratos.Include(x => x.Inmueble)
                                            .ThenInclude(x => x.Propietario)
                                            .Include(x => x.Inquilino)
                                            .Where(x => x.Inmueble.Propietario.Email == usuario)
                                            .Single(x => x.Id == id));

            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // POST api/<ContratosController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<ContratosController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ContratosController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
