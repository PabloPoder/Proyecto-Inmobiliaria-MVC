using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Proyecto_Inmobiliaria_MVC.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Proyecto_Inmobiliaria_MVC.api
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class PropietariosController : ControllerBase
    {

        private readonly DataContext contexto;
        private readonly IConfiguration config;

        public PropietariosController(DataContext contexto, IConfiguration config)
        {
            this.contexto = contexto;
            this.config = config;
        }

        // GET: api/<controller>
        [HttpGet]
        public async Task<ActionResult<Propietario>> Get()
        {
            try
            {
                var usuario = User.Identity.Name;

                return await contexto.Propietarios.SingleOrDefaultAsync(x => x.Email == usuario);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // GET: api/<controller>/5
        [HttpGet("{id}")]
        public async Task<ActionResult> Get(int id)
        {
            try
            {
                var entidad = await contexto.Propietarios.SingleOrDefaultAsync(x => x.Id == id);
                return entidad != null ? Ok(entidad) : NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // Get: api/<controller>/GetAll
        [HttpGet("GetAll")]
        [AllowAnonymous]
        public async Task<ActionResult> GetAll()
        {
            try
            {
                return Ok(await contexto.Propietarios.ToListAsync());
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // GET: api/<controller>/5
        [HttpGet("PropietarioActual")]
        public async Task<ActionResult> PropietarioActual()
        {
            try
            {
                var usuario = User.Identity.Name;
                var entidad = await contexto.Propietarios.SingleOrDefaultAsync(x => x.Email == usuario);

                return Ok(entidad);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // POST api/<PropietariosController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<PropietariosController>/5
        [HttpPut("EditarUsuario/{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] Propietario propietario)
        {
            try
            {
                if (ModelState.IsValid && contexto.Propietarios.AsNoTracking().SingleOrDefault(x => x.Id == id && x.Email == User.Identity.Name) != null) 
                {
                    propietario.Id = id;

                    string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                        password: propietario.Clave,
                        salt: System.Text.Encoding.ASCII.GetBytes(config["Salt"]),
                        prf: KeyDerivationPrf.HMACSHA1,
                        iterationCount: 1000,
                        numBytesRequested: 256 / 8));

                    propietario.Clave = hashed;

                    contexto.Propietarios.Update(propietario);
                    await contexto.SaveChangesAsync();
                    return Ok(propietario);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // DELETE api/<PropietariosController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var p = contexto.Propietarios.Find(id);
                    if (p == null) return NotFound();

                    contexto.Propietarios.Remove(p);
                    contexto.SaveChanges();
                    return Ok(p);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // POST api/<controller>/login
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromForm] LoginView loginView)
        {
            try
            {
                string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: loginView.Clave,
                    salt: System.Text.Encoding.ASCII.GetBytes(config["Salt"]),
                    prf: KeyDerivationPrf.HMACSHA1,
                    iterationCount: 1000,
                    numBytesRequested: 256 / 8));
                var p = await contexto.Propietarios.FirstOrDefaultAsync(x => x.Email == loginView.Usuario);

                if (p == null || p.Clave != hashed)
                {
                    return BadRequest("Nombre de usuario o clave incorrecta");
                }
                else
                {
                    var key = new SymmetricSecurityKey(
                        System.Text.Encoding.ASCII.GetBytes(config["TokenAuthentication:SecretKey"]));
                    var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, p.Email),
                        new Claim("FullName", p.Nombre + " " + p.Apellido),
                        new Claim(ClaimTypes.Role, "Propietario"),
                    };

                    var token = new JwtSecurityToken(
                        issuer: config["TokenAuthentication:Issuer"],
                        audience: config["TokenAuthentication:Audience"],
                        claims: claims,
                        expires: DateTime.Now.AddMinutes(60),
                        signingCredentials: credenciales
                    );
                    return Ok(new JwtSecurityTokenHandler().WriteToken(token));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
