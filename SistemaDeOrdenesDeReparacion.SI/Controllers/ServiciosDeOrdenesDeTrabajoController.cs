using Microsoft.AspNetCore.Mvc;
using SistemaDeOrdenesDeReparacion.BL;
using SistemaDeOrdenesDeReparacion.Model;


namespace SistemaDeOrdenesDeReparacion.SI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiciosDeOrdenesDeTrabajoController : ControllerBase
    {
        private readonly IGestionDeOrdenesDeTrabajo gestionDeOrdenesDeTrabajo;
        public ServiciosDeOrdenesDeTrabajoController(IGestionDeOrdenesDeTrabajo gestionDeOrdenesDeTrabajo)
        {
            this.gestionDeOrdenesDeTrabajo = gestionDeOrdenesDeTrabajo;
        }



        [HttpGet("ObtenerListaDeOrdenesDeTrabajo")]
        public IActionResult ObtenerListaDeOrdenesDeTrabajo()
        {
            return Ok(gestionDeOrdenesDeTrabajo.ObtenerTodasLasOrdenes());
        }

        [HttpGet("ObtenerOrdenPorNombre")]
        public IActionResult ObtenerOrdenPorNombre([FromQuery] string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                return BadRequest("Debe proporcionar un nombre.");
            }
            return Ok(gestionDeOrdenesDeTrabajo.FiltrarOrdenesPorNombre(nombre));
        }

        [HttpGet("PrepararFormularioDeVistaOrden")]
        public IActionResult PrepararFormularioDeVistaOrden()
        {
            var modelo = gestionDeOrdenesDeTrabajo.PrepararFormularioDeCreacion();
            gestionDeOrdenesDeTrabajo.PrepararDatosVistaOrden(modelo);
            return Ok(modelo);
        }




        [HttpPost("NuevaOrdenDeTrabajo")]
        public IActionResult NuevaOrdenDeTrabajo([FromBody] OrdenesDeTrabajoViewModel orden)
        {
            if (orden == null)
            {
                return BadRequest("Datos de la orden no válidos.");
            }
            if (gestionDeOrdenesDeTrabajo.AgregarOrden(orden))
            {
                return Ok("Orden de trabajo creada exitosamente.");
            }
            else
            {
                return BadRequest("Error al crear la orden de trabajo.");
            }
        }

        [HttpGet("EditarOrdenesDeTrabajo")]
        public IActionResult EditarOrdenesDeTrabajo([FromQuery] int id)
        {
            if (id <= 0)
                return BadRequest("ID inválido.");

            var orden = gestionDeOrdenesDeTrabajo.ObtenerOrdenParaEdicion(id);
            if (orden == null)
                return NotFound();

            var dto = new EditarOrdenesDeTrabajoDto
            {
                Orden = orden,
                Clientes = gestionDeOrdenesDeTrabajo.ObtenerListaClientesParaSeleccion()
                    .Select(c => new ClienteDto
                    {
                        Id = c.Id,
                        Nombre = $"{c.Identificacion} - {c.Nombre} {c.Apellidos}"
                    }).ToList(),
                TiposVehiculo = gestionDeOrdenesDeTrabajo.ObtenerTiposVehiculo(),
                TiposCombustion = gestionDeOrdenesDeTrabajo.ObtenerTiposCombustion()
            };

            return Ok(dto);
        }



        [HttpPut("ActualizarOrdenesDeTrabajo")]
        public IActionResult ActualizarOrdenesDeTrabajo([FromBody] OrdenesDeTrabajoViewModel orden)
        {
            if (orden == null || !gestionDeOrdenesDeTrabajo.OrdenExiste(orden.Id))
            {
                return BadRequest("Datos de la orden no válidos o la orden no existe.");
            }
            var entidad = gestionDeOrdenesDeTrabajo.ConvertirViewModelAEntidad(orden);
            if (gestionDeOrdenesDeTrabajo.ActualizarOrden(entidad))
            {
                return Ok("Orden actualizada exitosamente.");
            }
            else
            {
                return BadRequest("Error al actualizar la orden.");
            }

        }

        [HttpGet("CancelarOrdenesDeTrabajo")]
        public IActionResult CancelarOrdenesDeTrabajo([FromQuery] int id)
        {
            if (id <= 0)
            {
                return BadRequest("ID inválido.");
            }
            var modelo = gestionDeOrdenesDeTrabajo.PrepararFormularioDeCancelacion(id);
            if (modelo == null)
            {
                return NotFound("Orden no encontrada.");
            }
            return Ok(modelo);
        }

        [HttpPost("CancelarOrdenesDeTrabajo")]
        public IActionResult CancelarOrdenesDeTrabajo([FromBody] CancelarOrdenViewModel cancelacion)
        {
            if (cancelacion == null || !gestionDeOrdenesDeTrabajo.OrdenExiste(cancelacion.Id))
            {
                return BadRequest("Datos de cancelación no válidos o la orden no existe.");
            }
            if (gestionDeOrdenesDeTrabajo.CancelarOrden(cancelacion.Id, cancelacion.MotivoCancelacion))
            {
                return Ok("Orden cancelada exitosamente.");
            }
            else
            {
                return BadRequest("Error al cancelar la orden.");
            }
        }
        
        [HttpGet("ObtenerVistaParaOrdenesDeTrabajo")]
        public IActionResult ObtenerVistaParaOrdenesDeTrabajo([FromQuery] int id)
        {
            if (id <= 0)
            {
                return BadRequest("ID inválido.");
            }
            var modelo = gestionDeOrdenesDeTrabajo.PrepararFormularioDeInicio(id);
            if (modelo == null)
            {
                return NotFound("Orden no encontrada.");
            }
            return Ok(modelo);
        }

        [HttpPost("IniciarOrdenesDeTrabajo")]
        public IActionResult IniciarOrdenesDeTrabajo([FromBody] IniciarOrdenDto orden)
        {
            if (orden == null || orden.Id <= 0 || orden.MecanicoId <= 0)
                return BadRequest("Datos inválidos.");

            var resultado = gestionDeOrdenesDeTrabajo.IniciarOrden(orden.Id, orden.MecanicoId);
            return Ok(resultado);
        }


        [HttpGet("ObtenerClientesParaDropdown")]
        public IActionResult ObtenerClientesParaDropdown()
        {
            try
            {
               
                var clientesResumen = gestionDeOrdenesDeTrabajo.ObtenerListaClientesParaSeleccion();

                
                var clientesParaDropdown = clientesResumen.Select(c => new ClienteDto
                {
                    Id = c.Id,
                    Nombre = $"{c.Identificacion} - {c.Nombre} {c.Apellidos}"
                }).ToList();

              
                return Ok(clientesParaDropdown);
            }
            catch (Exception ex)
            {
               
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpGet("ObtenerTiposVehiculo")]
        public IActionResult ObtenerTiposVehiculo()
        {
            try
            {
                var tipos = gestionDeOrdenesDeTrabajo.ObtenerTiposVehiculo();
                return Ok(tipos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno al obtener tipos de vehículo: {ex.Message}");
            }
        }

        [HttpGet("ObtenerTiposCombustion")]
        public IActionResult ObtenerTiposCombustion()
        {
            try
            {
                var tipos = gestionDeOrdenesDeTrabajo.ObtenerTiposCombustion();
                return Ok(tipos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno al obtener tipos de combustión: {ex.Message}");
            }
        }


    }



}
