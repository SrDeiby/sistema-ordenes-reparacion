using Microsoft.AspNetCore.Mvc;
using SistemaDeOrdenesDeReparacion.BL;
using SistemaDeOrdenesDeReparacion.Model;
using static System.Runtime.InteropServices.JavaScript.JSType;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SistemaDeOrdenesDeReparacion.SI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServicioDeTrabajoMecanicoController : ControllerBase
    {

        private readonly IGestionLogin _gestionLogin;
        private readonly IGestionDeOrdenesDeTrabajo _gestionDeOrdenesDeTrabajo;
        private readonly ICatalogoDeRepuestos _gestionDeRepuestos;
        private readonly ICatalogoDeServicios _gestionDeServicios;

        public ServicioDeTrabajoMecanicoController(IGestionLogin gestionLogin, 
            IGestionDeOrdenesDeTrabajo gestionDeOrdenesDeTrabajo,
            ICatalogoDeRepuestos gestionDeRepuestos,
            ICatalogoDeServicios gestionDeServicios)
        {
            _gestionLogin = gestionLogin;
            _gestionDeOrdenesDeTrabajo = gestionDeOrdenesDeTrabajo;
            _gestionDeRepuestos = gestionDeRepuestos;
            _gestionDeServicios = gestionDeServicios;
        }

        [HttpGet("ObtengaLaListaDeOrdenesEnProceso")]
        public IActionResult ObtengaLaListaDeOrdenesEnProceso() { 
        
            return Ok(_gestionDeOrdenesDeTrabajo.ObtengaLaListaDeOrdenesEnProceso());
        }


        [HttpGet("FiltreLaListaDeOrdenes")]
        public IActionResult FiltreLaListaDeOrdenes(string filtro) 
        {
            return Ok(_gestionDeOrdenesDeTrabajo.FiltreOrdenesDeTrabajoPorOrdenes(filtro));
        }

        [HttpGet("ObtengaLaListaDeOrdenesCanceladas")]
        public IActionResult ObtengaLaListaDeOrdenesCanceladas()
        {

            return Ok(_gestionDeOrdenesDeTrabajo.ObtengaLaListaDeOrdenesCanceladas());
        }

        [HttpGet("FiltreLaListaDeOrdenesCanceladas")]
        public IActionResult FiltreLaListaDeOrdenesCanceladas(string filtro)
        {
            return Ok(_gestionDeOrdenesDeTrabajo.FiltreOrdenesDeTrabajoPorOrdenesCanceladas(filtro));
        }


        [HttpGet("FiltreRepuestoPorNombre")]
        public IActionResult FiltreRepuestoPorNombre(string filtro)
        {
            if (string.IsNullOrWhiteSpace(filtro))
            {
                return BadRequest("Debe proporcionar un nombre.");
            }
            return Ok(_gestionDeRepuestos.FiltrarRepuestoPorNombre(filtro));

        }

        [HttpGet("ObtengaLosRepuestos")]
        public IActionResult ObtengaLosRepuestos()
        {

            return Ok(_gestionDeRepuestos.ObtenerTodosLosRepuestos());

        }

        [HttpGet("FiltreServiciosPorNombre")]
        public IActionResult FiltreServiciosPorNombre(string filtro)
        {
            if (string.IsNullOrWhiteSpace(filtro))
            {
                return BadRequest("Debe proporcionar un nombre.");
            }
            return Ok(_gestionDeServicios.FiltrarServicioPorNombre(filtro));
        }

        [HttpGet("ObtengaLosServicios")]
        public IActionResult ObtengaLosServicios()
        {

            return Ok(_gestionDeServicios.ObtengaLaLista());

        }

        [HttpGet("RepuestoPorId")]
        public IActionResult RepuestoPorId(int id)
        {
            var repuesto = _gestionDeOrdenesDeTrabajo.ObtenerRepuestoPorId(id);
            if (repuesto == null)
                return NotFound();
            return Ok(repuesto);
        }

        [HttpGet("ServicioPorId")]
        public IActionResult ServicioPorId(int id)
        {
            var servicio = _gestionDeOrdenesDeTrabajo.ObtenerServicioPorId(id);
            if (servicio == null)
                return NotFound();
            return Ok(servicio);
        }

        [HttpGet("OrdenPorId")]
        public IActionResult OrdenPorId(int id)
        {
            var orden = _gestionDeOrdenesDeTrabajo.ObtenerOrdenPorId(id);
            if (orden == null)
                return NotFound();
            return Ok(orden);
        }

        [HttpGet("RepuestosAsignados")]
        public IActionResult RepuestosAsignados(int idOrden)
        {
            return Ok(_gestionDeOrdenesDeTrabajo.ObtengaLosRepuestosAsignados(idOrden));
        }

        [HttpGet("ServiciosAsignados")]
        public IActionResult ServiciosAsignados(int idOrden)
        {
            return Ok(_gestionDeOrdenesDeTrabajo.ObtengaLosServiciosAsignados(idOrden));
        }

        [HttpGet("ResumenDeOrden")]
        public IActionResult ResumenDeOrden(int idOrden)
        {
            var resumen = _gestionDeOrdenesDeTrabajo.ObtenerResumenDeOrden(idOrden);
            if (resumen == null)
                return NotFound();
            return Ok(resumen);
        }

        [HttpGet("OrdenCanceladaPorId")]
        public IActionResult OrdenCanceladaPorId(int idOrden)
        {
            var orden = _gestionDeOrdenesDeTrabajo.ObtengaElDetalleDeOrdenesCanceladas(idOrden);
            if (orden == null)
                return NotFound();
            return Ok(orden);
        }

        [HttpGet("OrdenesPorCliente")]
        public IActionResult OrdenesPorCliente(int idCliente)
        {
            return Ok(_gestionDeOrdenesDeTrabajo.ObtengaOrdenesPorIdCliente(idCliente));
        }


        [HttpPost("AsignarRepuesto")]
        public IActionResult AsignarRepuesto([FromBody] OrdenesDeTrabajoInventarioDeRepuestos model)
        {
            try
            {
                _gestionDeOrdenesDeTrabajo.AsigneRepuesto(model);
                return Ok(new { mensaje = "Repuesto asignado correctamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("AsignarServicio")]
        public IActionResult AsignarServicio([FromBody] OrdenesDeTrabajoInventarioDeServicios model)
        {
            try
            {
                _gestionDeOrdenesDeTrabajo.AsigneServicio(model);
                return Ok(new { mensaje = "Servicio asignado correctamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }







    }
}
