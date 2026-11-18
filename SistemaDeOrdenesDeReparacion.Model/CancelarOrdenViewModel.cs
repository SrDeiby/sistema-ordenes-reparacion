using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeOrdenesDeReparacion.Model
{
    public class CancelarOrdenViewModel
    {
        public int Id { get; set; }
        public string NumeroOrden { get; set; }
        public string Cliente { get; set; }
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "El motivo de cancelación es requerido")]
        [Display(Name = "Motivo de Cancelación")]
        public string MotivoCancelacion { get; set; }
    }
}
