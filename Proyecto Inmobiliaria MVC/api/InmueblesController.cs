using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Proyecto_Inmobiliaria_MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Proyecto_Inmobiliaria_MVC.Api
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class InmueblesController : ControllerBase
    {
        private readonly DataContext contexto;
        private readonly IConfiguration config;

        public InmueblesController(DataContext contexto, IConfiguration config)
        {
            this.contexto = contexto;
            this.config = config;
        }

        // GET: api/<InmueblesController>
        [HttpGet]
        public async Task<ActionResult> Inmuebles()
        {
            try
            {
                var usuario = User.Identity.Name;

                return Ok(contexto.Inmuebles.Include(x => x.Propietario).Where(x => x.Propietario.Email == usuario));
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // GET: api/<InmueblesController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult> Inmueble(int id)
        {
            try
            {
                var usuario = User.Identity.Name;

                return Ok(contexto.Inmuebles.Include(x => x.Propietario).Where(x => x.Propietario.Email == usuario).Single( x => x.Id == id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // POST api/<InmueblesController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<InmueblesController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<InmueblesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
