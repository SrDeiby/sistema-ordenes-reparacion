using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeOrdenesDeReparacion.Model
{
    public class OrdenesDeTrabajoViewModel
        
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Debe seleccionar un cliente.")]
        public int IdCliente { get; set; }

        [Required(ErrorMessage = "La fecha de finalización es obligatoria.")]
        [DataType(DataType.Date)]
        public DateTime FechaFinalizacion { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Debe seleccionar el tipo de vehículo.")]
        public int TipoDeVehiculo { get; set; }  

        [Required(ErrorMessage = "Debe seleccionar el tipo de combustión.")]
        public int TipoDeCombustionInterna { get; set; }  

        [Required(ErrorMessage = "La marca es obligatoria.")]
        public string Marca { get; set; }

        [Required(ErrorMessage = "El modelo es obligatorio.")]
        public string Modelo { get; set; }

        [Required(ErrorMessage = "El año de fabricación es obligatorio.")]
        [Range(1900, 2025, ErrorMessage = "El año debe estar entre 1900 y 2025.")]
        public string AñoFabricacion { get; set; }

        [Required(ErrorMessage = "Debe describir el problema.")]
        public string DescripcionProblema { get; set; }
    }
}