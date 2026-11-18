using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeOrdenesDeReparacion.Model
{
    public class Cliente
    {


        [Required(ErrorMessage = "La identificación es obligatoria.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo de nombre es obligatorio.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El campo de apellido es obligatorio.")]
        public string Apellidos { get; set; }

        [Required(ErrorMessage = "El campo de identificación es obligatorio.")]
        [Display(Name = "Identificación")]
        public string Identificacion { get; set; }

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "Debe ingresar un correo electrónico válido.")]
        [Display(Name = "Correo Electrónico")]
        public string? CorreoElectronico { get; set; }

      

    }
}
 