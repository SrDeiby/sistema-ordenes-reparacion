using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeOrdenesDeReparacion.Model.Response
{
    public class LoginResponse
    {

        public bool Exito { get; set; }

        public string Mensaje { get; set; }
        public string Rol { get; set; }
        public string Identificacion { get; set; } = string.Empty;
    }
}
