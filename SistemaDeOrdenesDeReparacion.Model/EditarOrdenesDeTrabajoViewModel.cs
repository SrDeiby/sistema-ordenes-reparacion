using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SistemaDeOrdenesDeReparacion.Model
{
    public class EditarOrdenesDeTrabajoViewModel
    {
        
        public OrdenesDeTrabajoViewModel Orden { get; set; }

        public SelectList Clientes { get; set; }
        public SelectList TiposVehiculo { get; set; }
        public SelectList TiposCombustion { get; set; }
    }
}