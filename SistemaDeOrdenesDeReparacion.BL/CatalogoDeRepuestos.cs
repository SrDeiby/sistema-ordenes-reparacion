

using System.Collections.Generic;

using System.Linq;

using SistemaDeOrdenesDeReparacion.DA;

namespace SistemaDeOrdenesDeReparacion.BL

{

    public class CatalogoDeRepuestos : ICatalogoDeRepuestos

    {

        private readonly DBContexto _context;

        public CatalogoDeRepuestos(DBContexto context)

        {

            _context = context;

        }

        public List<Model.InventarioDeRepuestos> ObtenerTodosLosRepuestos()



        {

            return _context.InventarioDeRepuestos.ToList();

        }







        public List<Model.InventarioDeRepuestos> FiltrarRepuestoPorNombre(string nombre)

        {
            return _context.InventarioDeRepuestos
                .Where(r => r.Nombre.ToLower().Contains(nombre.ToLower()))
                .ToList();
        }



        public void AgregarRepuesto(Model.InventarioDeRepuestos repuesto)

        {

            _context.InventarioDeRepuestos.Add(repuesto);

            _context.SaveChanges();

        }

        public void ActualizarRepuesto(Model.InventarioDeRepuestos repuesto)
        {
            var existente = _context.InventarioDeRepuestos.FirstOrDefault(r => r.Id == repuesto.Id);

            if (existente != null)
            {
                existente.Nombre = repuesto.Nombre;
                existente.Descripcion = repuesto.Descripcion;
                existente.PrecioUnitario = repuesto.PrecioUnitario;

                _context.SaveChanges();
            }
        }


        public Model.InventarioDeRepuestos BuscarPorId(int id)

        {

            return _context.InventarioDeRepuestos.Find(id);

        }

    }



}