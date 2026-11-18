using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeOrdenesDeReparacion.Model
{
    public class EditarOrdenesDeTrabajoDto
    {
        public OrdenesDeTrabajoViewModel Orden { get; set; }
        public List<ClienteDto> Clientes { get; set; }
        public Dictionary<string, string> TiposVehiculo { get; set; }

        public Dictionary<string, string> TiposCombustion { get; set; }
    }
}
