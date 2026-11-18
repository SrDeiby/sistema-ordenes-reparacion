using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeOrdenesDeReparacion.Model
{
    public class OrdenEnProcesoViewModel
    {
        public string IdentificacionCliente { get; set; }
        public string NombreCliente { get; set; }
        public string ApellidosCliente { get; set; }

        public string IdentificacionMecanico { get; set; }
        public string NombreMecanico { get; set; }
        public string ApellidosMecanico { get; set; }

        public string TipoDeVehiculo { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public string TipoDeCombustion { get; set; }
        public string AñoDeFabricacion { get; set; }
    }

}
