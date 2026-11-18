using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeOrdenesDeReparacion.Model.Request
{
    public class CambieContrasenaRequest
    {
        public int Id { get; set; }
        public string NuevaContrasena { get; set; } = string.Empty;
    }
}
