using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace SistemaDeOrdenesDeReparacion.Model
{
    public enum EstadoDeOrdenDeTrabajo
    {
        Registrado = 1,
        [Display(Name ="En Proceso")]
        EnProceso = 2,
        Finalizado = 3,
        Cancelado = 4
    }
}
