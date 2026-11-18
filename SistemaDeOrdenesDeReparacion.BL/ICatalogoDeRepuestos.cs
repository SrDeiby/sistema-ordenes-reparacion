using System.Collections.Generic;


namespace SistemaDeOrdenesDeReparacion.BL

{
    public interface ICatalogoDeRepuestos

    {

        List<Model.InventarioDeRepuestos> ObtenerTodosLosRepuestos();
        List<Model.InventarioDeRepuestos> FiltrarRepuestoPorNombre(string nombre);
        void AgregarRepuesto(Model.InventarioDeRepuestos repuesto);
        void ActualizarRepuesto(Model.InventarioDeRepuestos repuesto);
        Model.InventarioDeRepuestos BuscarPorId(int id);

    }



}