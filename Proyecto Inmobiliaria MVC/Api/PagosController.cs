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

namespace Proyecto_Inmobiliaria_MVC.api
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class PagosController : ControllerBase
    {

        private readonly DataContext contexto;
        private readonly IConfiguration config;

        public PagosController(DataContext contexto, IConfiguration config)
        {
            this.contexto = contexto;
            this.config = config;
        }


        // GET: api/<controller>
        [HttpGet("GetAll")]
        public async Task<ActionResult<Pago>> GetAll()
        {
            try
            {
                return Ok(await contexto.Pagos.ToListAsync());
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // GET api/<PagosController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Pago>> Get(int id)
        {
            try
            {
                return Ok(contexto.Pagos.Where(x => x.ContratoId == id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // POST api/<PagosController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<PagosController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<PagosController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
