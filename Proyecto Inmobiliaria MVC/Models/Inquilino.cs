using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Proyecto_Inmobiliaria_MVC.Models
{
    public class Inquilino
    {
        [Display(Name = "Código")]
        public int Id { get; set; }
        [Required]
        public string Nombre { get; set; }
        [Required]
        public string Apellido { get; set; }
        [Required]
        public string Dni{ get; set; }
        [Required]
        public string Telefono { get; set; }
        [Required, EmailAddress]
        public string Email { get; set; }
        public bool Estado { get; set; }

    }
}
