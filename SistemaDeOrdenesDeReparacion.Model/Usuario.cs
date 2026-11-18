using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeOrdenesDeReparacion.Model
{
    public class Usuario
    {

        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Contrasena { get; set; }

        public string? Identificacion { get; set; }

        public Tipo Tipo { get; set; }

        public Estado? Estado { get; set; }

        public int? IntentosFallidos { get; set; }

        public DateTime? FechaBloqueo { get; set; }

        public String? CorreoElectronico { get; set; }

    }
}
