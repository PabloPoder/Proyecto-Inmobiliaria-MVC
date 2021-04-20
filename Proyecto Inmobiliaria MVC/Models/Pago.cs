using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Proyecto_Inmobiliaria_MVC.Models
{
    public class Pago
    {
        [Display(Name = "Código")]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Fecha de Pago")]
        public DateTime FechaPago { get; set; }
        [Required]
        [Display(Name = "Código Inquilino")]
        [ForeignKey(nameof(ContratoId))]
        public int ContratoId { get; set; }
        public Contrato Contrato { get; set; }

    }
}
