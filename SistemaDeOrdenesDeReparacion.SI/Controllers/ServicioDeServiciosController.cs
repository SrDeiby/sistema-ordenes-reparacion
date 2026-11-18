using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata;
using SistemaDeOrdenesDeReparacion.Model;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SistemaDeOrdenesDeReparacion.SI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServicioDeServiciosController : ControllerBase
    {

        private readonly SistemaDeOrdenesDeReparacion.BL.ICatalogoDeServicios catalogoServicios;

        public ServicioDeServiciosController(SistemaDeOrdenesDeReparacion.BL.ICatalogoDeServicios servicios)
        {

            catalogoServicios = servicios;
        }

        [HttpGet("ObtengaLaLista")]
        public IActionResult ObtengaLaLista()
        {
            var lista = catalogoServicios.ObtengaLaLista();
            return Ok(lista); 
        }


        [HttpGet("ObtengaLaListaDeDisponibles")]
        public List<SistemaDeOrdenesDeReparacion.Model.InventarioDeServicios> ObtengaLaListaDeDisponibles()
        {
            return catalogoServicios.ObtengaLaListaDeDisponibles();
        }

        [HttpGet("ObtengaElServicio/{id}")]
        public SistemaDeOrdenesDeReparacion.Model.InventarioDeServicios ObtengaElServicio(int id)
        {
            return catalogoServicios.ObtengaELServicio(id);
        }

        [HttpPost("Agregue")]
        public IActionResult Agregue([FromBody] InventarioDeServicios servicio)
        {
            try
            {
                if (servicio == null)
                {
                    return BadRequest("El servicio no puede ser nulo.");
                }
                catalogoServicios.Agregue(servicio);
                return Ok("Servicio agregado exitosamente.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al agregar servicio: {ex.Message}");
            }
        }


        [HttpPut("EditeElServicio")]
        public IActionResult EditeElServicio([FromBody] SistemaDeOrdenesDeReparacion.Model.InventarioDeServicios servicio)
        {
            if (servicio == null || servicio.Id <= 0 ||
                string.IsNullOrWhiteSpace(servicio.Nombre) ||
                string.IsNullOrWhiteSpace(servicio.Descripcion))
            {
                return BadRequest("Datos del servicio incompletos o inválidos.");
            }

            try
            {
                var servicioExistente = catalogoServicios.ObtengaELServicio(servicio.Id);
                if (servicioExistente == null)
                {
                    return NotFound("Servicio no encontrado.");
                }

                catalogoServicios.EditeElServicio(servicio);
                return Ok("Servicio editado exitosamente.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al editar el servicio: {ex.Message}");
            }
        }



        [HttpGet("FiltrarServicioPorNombre")]
        public IActionResult FiltrarServicioPorNombre([FromQuery] string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                return BadRequest("Debe proporcionar un nombre.");
            }

            var listaFiltrada = catalogoServicios.FiltrarServicioPorNombre(nombre);
            return Ok(listaFiltrada); // Devuelve 200 con JSON
        }

    }
}
