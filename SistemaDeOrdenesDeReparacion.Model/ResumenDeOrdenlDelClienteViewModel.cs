using System.Collections.Generic;

namespace SistemaDeOrdenesDeReparacion.Model
{
    public class ResumenDeOrdenDelClienteViewModel
    {
        public OrdenesDeTrabajos Orden { get; set; }

        public List<OrdenesDeTrabajoInventarioDeRepuestos> Repuestos { get; set; }

        public List<OrdenesDeTrabajoInventarioDeServicios> Servicios { get; set; }

        public decimal TotalRepuestos { get; set; }

        public decimal TotalServicios { get; set; }

        public decimal TotalGeneral => TotalRepuestos + TotalServicios;
    }
}
