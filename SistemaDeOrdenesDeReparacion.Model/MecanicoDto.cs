using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeOrdenesDeReparacion.Model
{
    public class MecanicoDto
    {
        public int Id { get; set; }

        public string Identificacion { get; set; }

        public string Nombre { get; set; }
        public string Apellidos { get; set; }

        public string NombreCompleto => $" {Identificacion} - {Nombre} {Apellidos}";






    }
}
