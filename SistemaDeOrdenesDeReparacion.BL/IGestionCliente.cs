using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SistemaDeOrdenesDeReparacion.Model;

namespace SistemaDeOrdenesDeReparacion.BL
{
    public  interface IGestionCliente
    {

        List<Model.Cliente> obtengaCliente();

        List<Model.Cliente> FiltrarClientesPorNombre(string nombre);


        void EditeElCliente(int id, string identificacion, string nombre, string apellidos, string correoelectronico);

        void envieCorreoElectronico(string correoElectronico, string asunto, string mensaje);

        void RegistreClienteConCredenciales(Cliente cliente);

        Cliente ObtengaClientePorId(int id);




    }
}
