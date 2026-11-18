using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeOrdenesDeReparacion.Model
{
    public class OrdenesDeTrabajoInventarioDeRepuestos
    {
        [Key]
        public int Id { get; set; }

        public int Id_OrdenesDeTrabajos { get; set; }

        public int Id_InventarioDeRepuestos { get; set; }

       
        [Range(1, 100, ErrorMessage = "Debe ser al menos 1")]
        [Required(ErrorMessage = "La cantidad es obligatoria.")]
        
        public int Cantidad { get; set; }

        public decimal Total { get; set; }

        [ForeignKey("Id_InventarioDeRepuestos")]
        public virtual InventarioDeRepuestos? InventarioDeRepuesto { get; set; }

    }
}
