using Microsoft.EntityFrameworkCore;
using SistemaDeOrdenesDeReparacion.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeOrdenesDeReparacion.BL
{
    public class CatalogoDeServicios : ICatalogoDeServicios
    {

        private SistemaDeOrdenesDeReparacion.DA.DBContexto ElContextoDeBaseDeDatos;

        public CatalogoDeServicios(SistemaDeOrdenesDeReparacion.DA.DBContexto dBContexto)
        {
            ElContextoDeBaseDeDatos = dBContexto;
        }

        public void Agregue(Model.InventarioDeServicios servicio)
        {
            ElContextoDeBaseDeDatos.InventarioDeServicios.Add(servicio);
            ElContextoDeBaseDeDatos.SaveChanges();
        }

        public Model.InventarioDeServicios ObtengaElServicio(int id)
        {
            List<Model.InventarioDeServicios> lista;
            lista = ObtengaLaLista();
            foreach (var servicio in lista)
            {
                if (servicio.Id == id)
                    return servicio;
            }

            return null;
        }

        public void EditeElServicio(Model.InventarioDeServicios servicio)
        {
            Model.InventarioDeServicios ElServicioAEditar;
            ElServicioAEditar = ObtengaELServicio(servicio.Id);

            ElServicioAEditar.Nombre = servicio.Nombre;
            ElServicioAEditar.Descripcion = servicio.Descripcion;
            ElServicioAEditar.Precio = servicio.Precio;
            ElContextoDeBaseDeDatos.InventarioDeServicios.Update(ElServicioAEditar);
            ElContextoDeBaseDeDatos.SaveChanges();
        }

        public Model.InventarioDeServicios ObtengaELServicio(int id)
        {
            List<Model.InventarioDeServicios> lista;
            lista = ObtengaLaLista();
            foreach (var servicio in lista)
            {
                if (servicio.Id == id)
                    return servicio;
            }

            return null;
        }

        public List<Model.InventarioDeServicios> ObtengaLaLista()
        {
            var resultado = from c in ElContextoDeBaseDeDatos.InventarioDeServicios
                            select c;
            return resultado.ToList();
        }

        public List<InventarioDeServicios> ObtengaLaListaDeDisponibles()
        {
            throw new NotImplementedException();
        }

        public List<Model.InventarioDeServicios> FiltrarServicioPorNombre(string nombre)

        {
            return ElContextoDeBaseDeDatos.InventarioDeServicios
                .Where(r => r.Nombre.ToLower().Contains(nombre.ToLower()))
                .ToList();
        }



    }
}
