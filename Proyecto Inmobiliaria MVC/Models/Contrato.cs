using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Proyecto_Inmobiliaria_MVC.Models
{
    public class Contrato
    {
        [Display(Name = "Código")]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Fecha Desde")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FechaDesde { get; set; }
        [Required]
        [Display(Name = "Fecha Hasta")]
        [DataType(DataType.Date)]
        public DateTime FechaHasta { get; set; }
        [Required]
        public Inquilino Inquilino { get; set; }
        [Required]
        public Inmueble Inmueble { get; set; }
        [Required]
        [Display(Name = "Código Inmueble")]
        [ForeignKey(nameof(InmuebleId))]
        public int InmuebleId { get; set; }
        [Required]
        [Display(Name = "Código Inquilino")]
        [ForeignKey(nameof(InquilinoId))]
        public int InquilinoId { get; set; }
        public bool Estado { get; set; }

    }
}
