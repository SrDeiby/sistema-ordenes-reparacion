using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeOrdenesDeReparacion.Model
{
    public class ResumenDeCostoViewModel
    {
        public decimal TotalRepuestos { get; set; }
        public decimal TotalServicios { get; set; }
        public decimal Total => TotalRepuestos + TotalServicios;
    }
}