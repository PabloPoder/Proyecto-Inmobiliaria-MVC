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
        
        [Display(Name = "Fecha de Pago")]
        [Required ,DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FechaPago { get; set; }
        [Required]
        [Display(Name = "Código Contrato")]
        [ForeignKey(nameof(ContratoId))]
        public int ContratoId { get; set; }
        public Contrato Contrato { get; set; }
        [Required]
        public Decimal Precio { get; set; }
        [Required]
        public bool Estado { get; set; }

    }
}
