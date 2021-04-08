using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Proyecto_Inmobiliaria_MVC.Models
{
    public class Contrato
    {
        [Display(Name = "Código")]
        public int Id { get; set; }
        [Required]
        public Inquilino Inquilino { get; set; }
        [Required]
        public Inmueble Inmueble { get; set; }
        [Required]
        public DateTime FechaDesde { get; set; }
        [Required]
        public DateTime FechaHasta { get; set; }
    }
}
