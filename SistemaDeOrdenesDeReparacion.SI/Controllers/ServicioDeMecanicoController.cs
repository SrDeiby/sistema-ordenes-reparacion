using Microsoft.AspNetCore.Mvc;
using SistemaDeOrdenesDeReparacion.BL;
using SistemaDeOrdenesDeReparacion.Model;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SistemaDeOrdenesDeReparacion.SI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServicioDeMecanicoController : ControllerBase
    {
        private readonly IGestionMecanicos gestionMecanicos;
     

        public ServicioDeMecanicoController(IGestionMecanicos gestionMecanicos)
        {
            this.gestionMecanicos = gestionMecanicos;
          
        }

        [HttpGet("ObtenerListaDeMecanicos")]
        public IActionResult ObtenerListaDeMecanicos()
        {
            return Ok(gestionMecanicos.obtengaMecanicos());
        }

        [HttpGet("ObtenerMecanicoPorNombre")]
        public IActionResult ObtenerMecanicoPorNombre([FromQuery] string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                return BadRequest("Debe proporcionar un nombre.");
            }
            return Ok(gestionMecanicos.FiltrarMecanicosPorNombre(nombre));
        }

        [HttpGet("ObtenerMecanicoPorId")]
        public IActionResult ObtenerMecanicoPorId([FromQuery] int id)
        {
            if (id <= 0)
            {
                return BadRequest("ID inválido.");
            }
            var mecanico = gestionMecanicos.ObtengaMecanicoPorId(id);
            if (mecanico == null)
            {
                return NotFound("Mecánico no encontrado.");
            }
            return Ok(mecanico);
        }




        [HttpPost("RegistrarMecanico")]
        public IActionResult RegistrarMecanico([FromBody] Mecanico mecanico)
        {
            if (mecanico == null || string.IsNullOrWhiteSpace(mecanico.Identificacion)
                || string.IsNullOrWhiteSpace(mecanico.Nombre) || string.IsNullOrWhiteSpace(mecanico.Apellidos))
            {
                return BadRequest("Datos del mecánico incompletos.");
            }

            try
            {
                gestionMecanicos.RegistreMecanicoConCredenciales(mecanico);
                return Ok("Mecánico registrado correctamente.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al registrar el mecánico: {ex.Message}");
            }
        }

        [HttpPut("EditarMecanico")]
        public IActionResult EditarMecanico([FromBody] Mecanico mecanico)
        {
            if (mecanico == null || mecanico.Id <= 0 ||
                string.IsNullOrWhiteSpace(mecanico.Identificacion) ||
                string.IsNullOrWhiteSpace(mecanico.Nombre) ||
                string.IsNullOrWhiteSpace(mecanico.Apellidos) ||
                string.IsNullOrWhiteSpace(mecanico.CorreoElectronico))
            {
                return BadRequest("Datos del mecánico incompletos.");
            }

            try
            {
                gestionMecanicos.EditeElMecanico(
                    mecanico.Id,
                    mecanico.Identificacion,
                    mecanico.Nombre,
                    mecanico.Apellidos,
                    mecanico.CorreoElectronico
                );

                return Ok("Mecánico editado correctamente.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al editar el mecánico: {ex.Message}");
            }
        }





    }
}
