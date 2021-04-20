using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Proyecto_Inmobiliaria_MVC.Models
{
    public class Inmueble
    {
        [Display(Name = "Código")]
        public int Id{ get; set; }
        [Required]
        public string Direccion { get; set; }
        [Required]
        public int Ambientes { get; set; }
        [Required]
        public int Superficie { get; set; }
        [Required]
        public decimal Latitud { get; set; }
        [Required]
        public decimal Longitud { get; set; }
        [Required]
        public decimal Precio { get; set; }
        [Display(Name = "Dueño")]
        public int PropietarioId { get; set; }
        [ForeignKey(nameof(PropietarioId))]
        public Propietario Propietario { get; set; }
    }
}
