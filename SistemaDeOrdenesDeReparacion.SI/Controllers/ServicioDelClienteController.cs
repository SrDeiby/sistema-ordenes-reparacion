using Microsoft.AspNetCore.Mvc;
using SistemaDeOrdenesDeReparacion.BL;
using SistemaDeOrdenesDeReparacion.Model;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SistemaDeOrdenesDeReparacion.SI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServicioDelClienteController : ControllerBase
    {
        // GET: api/<ServicioDelClienteController>

        private readonly IGestionCliente  gestionCliente;


        public ServicioDelClienteController(IGestionCliente gestionCliente)
        {
            this.gestionCliente = gestionCliente;

        }
        [HttpGet("ObtenerListaDeClientes")]
        public IActionResult ObtenerListaClientes()
        {
            return Ok(gestionCliente.obtengaCliente());
        }

        [HttpGet("ObtenerClientePorNombre")]
        public IActionResult ObtenerClientePorNombre([FromQuery] string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                return BadRequest("Debe proporcionar un nombre.");
            }
            return Ok(gestionCliente.FiltrarClientesPorNombre(nombre));
        }

        [HttpGet("ObtenerClientePorId")]
        public IActionResult ObtenerClientePorId([FromQuery] int id)
        {
            if (id <= 0)
            {
                return BadRequest("ID inválido.");
            }
            var cliente = gestionCliente.ObtengaClientePorId(id);
            if (cliente == null)
            {
                return NotFound("Cliente no encontrado.");
            }
            return Ok(cliente);
        }




        [HttpPost("RegistrarCliente")]
        public IActionResult RegistrarCliente([FromBody] Cliente cliente)
        {
            if (cliente == null || string.IsNullOrWhiteSpace(cliente.Identificacion)
                || string.IsNullOrWhiteSpace(cliente.Nombre) || string.IsNullOrWhiteSpace(cliente.Apellidos))
            {
                return BadRequest("Datos del cliente incompletos.");
            }

            try
            {
                gestionCliente.RegistreClienteConCredenciales(cliente);
                return Ok("Cliente registrado correctamente.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al registrar el cliente: {ex.Message}");
            }
        }

        [HttpPut("EditarCliente")]
        public IActionResult EditarCliente([FromBody] Cliente cliente)
        {
            if (cliente == null || cliente.Id <= 0 ||
                string.IsNullOrWhiteSpace(cliente.Identificacion) ||
                string.IsNullOrWhiteSpace(cliente.Nombre) ||
                string.IsNullOrWhiteSpace(cliente.Apellidos) ||
                string.IsNullOrWhiteSpace(cliente.CorreoElectronico))
            {
                return BadRequest("Datos del cliente incompletos.");
            }

            try
            {
                gestionCliente.EditeElCliente(
                    cliente.Id,
                    cliente.Identificacion,
                    cliente.Nombre,
                    cliente.Apellidos,
                    cliente.CorreoElectronico
                );

                return Ok("Cliente editado correctamente.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al editar el cliente: {ex.Message}");
            }
        }



    }
}
