using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeOrdenesDeReparacion.Model
{
    public class OrdenesDeTrabajos
    {
        public int Id { get; set; }
        [Column("Id_Cliente")]
        public int IdCliente { get; set; }
        public string DescripcionDelProblema { get; set; }
        public DateTime FechaDeRegistro { get; set; }
        public DateTime? FechaAproximadaDeFinalizacion { get; set; }
        public EstadoDeOrdenDeTrabajo Estado { get; set; }
        [ForeignKey ("Id_Mecanico")]
        public virtual Mecanico? Mecanico { get; set; }
        public int? Id_Mecanico  { get; set; }
        public TipoDeVehiculo TipoDeVehiculo { get; set; }
        public TipoDeCombustion TipoDeCombustionInterna { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }


        [Required(ErrorMessage = "El año de fabricación es obligatorio.")]
        [Range(1900, 2025, ErrorMessage = "El año debe estar entre 1900 y 2025.")]
        public string AñoDeFabricacion { get; set; }
        public string? MotivoDeCancelacion { get; set; }
        [ForeignKey("IdCliente")]
        public virtual Cliente Cliente { get; set; }





    }
}
