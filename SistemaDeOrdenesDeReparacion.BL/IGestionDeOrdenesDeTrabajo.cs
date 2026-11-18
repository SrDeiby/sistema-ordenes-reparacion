using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using SistemaDeOrdenesDeReparacion.Model;

namespace SistemaDeOrdenesDeReparacion.BL
{

    public interface IGestionDeOrdenesDeTrabajo
    {
        List<ClienteResumen> ObtenerListaClientesParaSeleccion();
        Dictionary<string, string> ObtenerTiposVehiculo();   
        Dictionary<string, string> ObtenerTiposCombustion();  
        bool AgregarOrden(OrdenesDeTrabajoViewModel request);
        CrearOrdenViewModel PrepararFormularioDeCreacion();
        List<ResumenDeOrdenesViewModel> ObtenerTodasLasOrdenes();
        List<ResumenDeOrdenesViewModel> FiltrarOrdenesPorNombre(string nombre);
        
        bool OrdenExiste(int id);
        bool ActualizarOrden(OrdenesDeTrabajos entidad);

        OrdenesDeTrabajos ConvertirViewModelAEntidad(OrdenesDeTrabajoViewModel vm);
        EditarOrdenesDeTrabajoViewModel PrepararFormularioDeEdicion(int id);

        CancelarOrdenViewModel PrepararFormularioDeCancelacion(int id);
        bool CancelarOrden(int id, string motivoCancelacion);
        void PrepararDatosVistaOrden(CrearOrdenViewModel modelo);
        OrdenesDeTrabajos ObtenerOrdenPorId(int id);
    
        bool IniciarOrden(int ordenId, int mecanicoId);
        IniciarOrdenViewModel PrepararFormularioDeInicio(int id);
        List<SelectListItem> ObtenerMecanicos();

        List<Model.OrdenesDeTrabajos> ObtengaLaListaDeOrdenesEnProceso();
        List<OrdenEnProcesoViewModel> ObtenerOrdenesEnProcesoParaVista();

        List<Model.OrdenesDeTrabajos> ObtengaLaListaDeOrdenesCanceladas();
        List<Model.OrdenesDeTrabajos> ObtengaLaLista();

        List<InventarioDeRepuestos> ObtengaLosRepuestos();
        List<InventarioDeServicios> ObtengaLosServicios();

        Model.InventarioDeServicios ObtenerServicioPorId(int id);
        Model.InventarioDeRepuestos ObtenerRepuestoPorId(int id);
        void Agregue(OrdenesDeTrabajoInventarioDeServicios automovil);

        void AgregueRepuesto(OrdenesDeTrabajoInventarioDeRepuestos automovil);
        void AsigneServicio(Model.OrdenesDeTrabajoInventarioDeServicios servicios);

        void AsigneRepuesto(Model.OrdenesDeTrabajoInventarioDeRepuestos repuestos);

        List<OrdenesDeTrabajoInventarioDeRepuestos> ObtengaLosRepuestosAsignados(int idOrden);

        List<OrdenesDeTrabajoInventarioDeServicios> ObtengaLosServiciosAsignados(int idOrden);

        Model.OrdenesDeTrabajos ObtengaElDetalleDeOrdenesCanceladas(int id);

        List<Model.OrdenesDeTrabajos> ObtengaOrdenesPorIdCliente(int idCliente);

        Model.OrdenesDeTrabajos ObtengaLaOrdenPorId(int idOrden);

        ResumenDeCostoViewModel ObtenerResumenDeOrden(int idOrden);

        List<OrdenesDeTrabajos> FiltreOrdenesDeTrabajoPorOrdenes(string nombre);
        List<OrdenesDeTrabajos> FiltreOrdenesDeTrabajoPorOrdenesCanceladas(string nombre);

        OrdenesDeTrabajoViewModel ObtenerOrdenParaEdicion(int id);
    }
}