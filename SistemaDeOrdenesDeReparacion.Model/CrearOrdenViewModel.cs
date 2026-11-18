using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SistemaDeOrdenesDeReparacion.Model
{
    public class CrearOrdenViewModel
    {
        public OrdenesDeTrabajoViewModel Orden { get; set; }

        public List<SelectListItem> Clientes { get; set; } = new();
        public List<SelectListItem> TiposVehiculo { get; set; } = new();
        public List<SelectListItem> TiposCombustion { get; set; } = new();

    
    }
}
