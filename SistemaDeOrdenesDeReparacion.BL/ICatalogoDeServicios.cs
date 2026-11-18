using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeOrdenesDeReparacion.BL
{
    public interface ICatalogoDeServicios
    {
        void Agregue(Model.InventarioDeServicios servicio); //YA

        List<Model.InventarioDeServicios> ObtengaLaLista(); //YA
        List<Model.InventarioDeServicios> ObtengaLaListaDeDisponibles(); //YA
        Model.InventarioDeServicios ObtengaELServicio(int id); //YA

        Model.InventarioDeServicios ObtengaElServicio(int id);
        void EditeElServicio(Model.InventarioDeServicios servicio); //YA

        List<Model.InventarioDeServicios> FiltrarServicioPorNombre(string nombre); //YA

    }
}
