using System;
using SistemaDeOrdenesDeReparacion.Model;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace SistemaDeOrdenesDeReparacion.BL
{
    public interface IGestionMecanicos
    {
      
        List<Model.Mecanico> obtengaMecanicos();

        List<Model.Mecanico> FiltrarMecanicosPorNombre(string Nombre);

        void EditeElMecanico(int id, string identificacion, string nombre, string apellidos, string correoelectronico);

        void envieCorreoElectronico(string correoElectronico, string asunto, string mensaje);

        void RegistreMecanicoConCredenciales(Model.Mecanico mecanico);

        public Mecanico ObtengaMecanicoPorId(int id);




    }
}
