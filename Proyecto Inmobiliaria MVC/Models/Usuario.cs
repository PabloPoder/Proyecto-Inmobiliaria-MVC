using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Proyecto_Inmobiliaria_MVC.Models
{
    public enum enRoles
    {
        SuperAdministrador = 1,
        Administrador = 2,
        Empleado = 3,
    }

    public class Usuario
    {
        [Display(Name = "Código")]
        public int Id{ get; set; }
        [Required]
        public string Nombre { get; set; }
        [Required, DataType(DataType.Text), StringLength(16, MinimumLength = 4)]
        public string Apellido { get; set; }
        [Required, DataType(DataType.EmailAddress), StringLength(50, MinimumLength = 8)]
        public string Email { get; set; }
        [Required, DataType(DataType.Password), StringLength(16, MinimumLength = 8)]
        public string Clave{ get; set; }
        public string Avatar { get; set; }
        [Display(Name = "Avatar")]
        [NotMapped]
        public IFormFile AvatarFile { get; set; }
        public bool Estado { get; set; }
        public int Rol { get; set; }

        public string RolNombre => Rol > 0 ? ((enRoles)Rol).ToString() : "";

        public static IDictionary<int, string> ObtenerRoles()
        {
            SortedDictionary<int, string> roles = new SortedDictionary<int, string>();
            Type tipoEnumRol = typeof(enRoles);
            foreach (var valor in Enum.GetValues(tipoEnumRol))
            {
                roles.Add((int)valor, Enum.GetName(tipoEnumRol, valor));
            }
            return roles;
        }
    }
}
