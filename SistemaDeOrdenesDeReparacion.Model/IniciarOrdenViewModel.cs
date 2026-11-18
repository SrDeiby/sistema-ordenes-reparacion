using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeOrdenesDeReparacion.Model
{
    public class IniciarOrdenViewModel
{
    public int Id { get; set; }
    public string TipoVehiculo { get; set; }
    public string Marca { get; set; }
    public string Modelo { get; set; }
    public string TipoCombustion { get; set; }
    public String AnioFabricacion { get; set; }
    public string DescripcionProblema { get; set; }

    [Required(ErrorMessage = "Debe seleccionar un mecánico.")] 
    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un mecánico.")]
    public int MecanicoId { get; set; }

    public SelectList Mecanicos { get; set; }
    }

}
