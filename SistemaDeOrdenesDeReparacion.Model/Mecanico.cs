using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SistemaDeOrdenesDeReparacion.Model
{
    public class Mecanico
    {

        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Los apellidos son obligatorios.")]
        public string Apellidos { get; set; }

        [Required(ErrorMessage = "La identificación es obligatoria.")]
        [Display(Name = "Identificación")]
        public string Identificacion { get; set; }

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "Debe ingresar un correo electrónico válido.")]
        [Display(Name = "Correo Electrónico")]
        public string? CorreoElectronico { get; set; }

    }
}
