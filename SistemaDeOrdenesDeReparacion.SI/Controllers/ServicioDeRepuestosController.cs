using Microsoft.AspNetCore.Mvc;
using SistemaDeOrdenesDeReparacion.Model;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SistemaDeOrdenesDeReparacion.SI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServicioDeRepuestosController : ControllerBase
    {
        private readonly SistemaDeOrdenesDeReparacion.BL.ICatalogoDeRepuestos catalogoDeRepuestos;

        public ServicioDeRepuestosController(SistemaDeOrdenesDeReparacion.BL.ICatalogoDeRepuestos repuestos)
        {

            catalogoDeRepuestos = repuestos;
        }

        [HttpGet("ObtengaLaLista")]
        public IActionResult ObtengaLaLista()
        {
            var lista = catalogoDeRepuestos.ObtenerTodosLosRepuestos();
            return Ok(lista);
        }



        [HttpGet("BuscarPorId/{id}")]
        public SistemaDeOrdenesDeReparacion.Model.InventarioDeRepuestos BuscarPorId(int id)
        {
            return catalogoDeRepuestos.BuscarPorId(id);
        }

        [HttpPost("AgregarRepuesto")]
        public IActionResult AgregarRepuesto([FromBody] InventarioDeRepuestos repuestos)
        {
            try
            {
                if (repuestos == null)
                {
                    return BadRequest("El repuesto no puede ser nulo.");
                }
                catalogoDeRepuestos.AgregarRepuesto(repuestos);
                return Ok("Repuesto agregado exitosamente.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al agregar repuesto: {ex.Message}");
            }
        }


        [HttpPut("ActualizarRepuesto")]
        public IActionResult ActualizarRepuesto([FromBody] SistemaDeOrdenesDeReparacion.Model.InventarioDeRepuestos repuesto)
        {
            if (repuesto == null || repuesto.Id <= 0 ||
                string.IsNullOrWhiteSpace(repuesto.Nombre) ||
                string.IsNullOrWhiteSpace(repuesto.Descripcion))
            {
                return BadRequest("Datos del repuesto incompletos o inválidos.");
            }

            try
            {
                var servicioExistente = catalogoDeRepuestos.BuscarPorId(repuesto.Id);
                if (servicioExistente == null)
                {
                    return NotFound("Repuesto no encontrado.");
                }

                catalogoDeRepuestos.ActualizarRepuesto(repuesto);
                return Ok("Repuesto editado exitosamente.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al editar el repuesto: {ex.Message}");
            }
        }


        [HttpGet("FiltrarRepuestoPorNombre")]
        public List<SistemaDeOrdenesDeReparacion.Model.InventarioDeRepuestos> FiltrarRepuestoPorNombre(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                return catalogoDeRepuestos.ObtenerTodosLosRepuestos();
            }
            return catalogoDeRepuestos.FiltrarRepuestoPorNombre(nombre);

        }
    }
}
