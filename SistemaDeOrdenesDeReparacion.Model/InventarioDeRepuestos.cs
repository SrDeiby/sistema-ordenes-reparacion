using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeOrdenesDeReparacion.Model

{

    public class InventarioDeRepuestos
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]

        public string Nombre { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria")]
        [Display(Name = "Descripción")]

        public string Descripcion { get; set; }

        [Required(ErrorMessage = "El precio es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El campo Precio debe ser mayor que 0")]
        [Display(Name = "Precio Unitario")]
        public decimal PrecioUnitario { get; set; }

    }

}