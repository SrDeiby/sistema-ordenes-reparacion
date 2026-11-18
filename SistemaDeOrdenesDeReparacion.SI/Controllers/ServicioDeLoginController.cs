using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SistemaDeOrdenesDeReparacion.SI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServicioDeLoginController : ControllerBase
    {
        private readonly SistemaDeOrdenesDeReparacion.BL.IGestionLogin gestionLogin;

        public ServicioDeLoginController(SistemaDeOrdenesDeReparacion.BL.IGestionLogin login)
        {

            gestionLogin = login;
        }

        
        [HttpPost("IniciarSesion")]
        public IActionResult IniciarSesion([FromBody] Model.Request.LoginRequest peticionLogin)
        {
            if (string.IsNullOrWhiteSpace(peticionLogin.Nombre) || string.IsNullOrWhiteSpace(peticionLogin.Contrasena))
            {
                return BadRequest(new SistemaDeOrdenesDeReparacion.Model.Response.LoginResponse
                {
                    Exito = false,
                    Mensaje = "El nombre de usuario y la clave son obligatorios."
                });
            }
            try
            {
                var administrador = gestionLogin.InicieSesionAdministrador(peticionLogin.Nombre, peticionLogin.Contrasena);
                if (administrador != null)
                {
                    return Ok(new SistemaDeOrdenesDeReparacion.Model.Response.LoginResponse
                    {
                        Exito = true,
                        Mensaje = "Inicio de sesión exitoso.",
                        Rol = "Administrador",

                    });
                }

                if (gestionLogin.SiElUsuarioSeBloqueaDespuesDeDosIntentos(peticionLogin.Nombre, peticionLogin.Contrasena))
                {
                    return Ok(new SistemaDeOrdenesDeReparacion.Model.Response.LoginResponse
                    {
                        Exito = false,
                        Mensaje = "El usuario está bloqueado debido a múltiples intentos fallidos. Por favor, intente más tarde."
                    });


                }

                if (gestionLogin.InicieUnaSesionDelUsuario(peticionLogin.Nombre, peticionLogin.Contrasena))
                {

                    var verificarUsuario = gestionLogin.VerificarCredenciales(peticionLogin.Nombre, peticionLogin.Contrasena);
                    if (verificarUsuario != null)
                    {
                        var rol = verificarUsuario.Rol.ToString().ToLowerInvariant();
                        return Ok(new SistemaDeOrdenesDeReparacion.Model.Response.LoginResponse
                        {
                            Exito = true,
                            Mensaje = "Inicio de sesión exitoso.",
                            Rol = rol,
                            Identificacion = verificarUsuario.Identificacion,
                        });
                    }

                }

                return Ok(new SistemaDeOrdenesDeReparacion.Model.Response.LoginResponse
                {
                    Exito = false,
                    Mensaje = "Nombre de usuario o contraseña incorrectos."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new SistemaDeOrdenesDeReparacion.Model.Response.LoginResponse
                {
                    Exito = false,
                    Mensaje = $"Error interno del servidor: {ex.Message}"
                });
            }
        }

        [HttpGet("ObtengaClientePorIdentificacion/{identificacion}")]
        public IActionResult ObtengaClientePorIdentificacion(string identificacion)
        {
            try
            {
                var cliente = gestionLogin.ObtengaClientePorIdentificacionDelUsuario(identificacion);
                if (cliente == null)
                {
                    return NotFound(new { Mensaje = "Cliente no encontrado." });
                }
                return Ok(cliente);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Mensaje = $"Error interno del servidor: {ex.Message}" });
            }
        }

        [HttpGet("ObtengaMecanicoPorIdentificacion/{identificacion}")]
        public IActionResult ObtengaMecanicoPorIdentificacion(string identificacion)
        {
            try
            {
                var mecanico = gestionLogin.ObtengaMecanicoPorIdentificacionDelUsuario(identificacion);
                if (mecanico == null)
                {
                    return NotFound(new { Mensaje = "Mecánico no encontrado." });
                }
                return Ok(mecanico);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Mensaje = $"Error interno del servidor: {ex.Message}" });
            }
        }

        [HttpGet("ObtengaUsuarioPorIdentificacionDelCliente/{identificacion}")]
        public IActionResult ObtengaUsuarioPorIdentificacionDelCliente(string identificacion)
        {
            try
            {
                var usuario = gestionLogin.ObtengaUsuarioPorIdentificacionDelCliente(identificacion);
                if (usuario == null)
                {
                    return NotFound(new { Mensaje = "Usuario no encontrado." });
                }
                return Ok(usuario);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Mensaje = $"Error interno del servidor: {ex.Message}" });
            }
        }

        [HttpGet("ObtengaUsuarioPorIdentificacionDelMecanico/{identificacion}")]
        public IActionResult ObtengaUsuarioPorIdentificacionDelMecanico(string identificacion)
        {
            try
            {
                var usuario = gestionLogin.ObtengaUsuarioPorIdentificacionDelMecanico(identificacion);
                if (usuario == null)
                {
                    return NotFound(new { Mensaje = "Usuario no encontrado." });
                }
                return Ok(usuario);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Mensaje = $"Error interno del servidor: {ex.Message}" });
            }
        }

        [HttpGet("ObtengaUsuarioPorId/{id}")]
        public IActionResult ObtengaUsuarioPorId(int id)
        {
            try
            {
                var usuario = gestionLogin.ObtengaUsuarioPorId(id);
                if (usuario == null)
                {
                    return NotFound(new { Mensaje = "Usuario no encontrado." });
                }
                return Ok(usuario);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Mensaje = $"Error interno del servidor: {ex.Message}" });
            }
        }


        [HttpPost("CambieContrasenaDelUsuario")]
        public IActionResult CambieContrasenaDelUsuario([FromBody] SistemaDeOrdenesDeReparacion.Model.Request.CambieContrasenaRequest request)
        {
            if (request.Id <= 0 || string.IsNullOrWhiteSpace(request.NuevaContrasena))
            {
                return BadRequest(new { Mensaje = "ID de usuario y nueva contraseña son obligatorios." });
            }
            try
            {
                var exito = gestionLogin.CambieLaContrasenaDelUsuario(request.Id, request.NuevaContrasena);
                if (exito)
                {
                    return Ok(new { Mensaje = "Contraseña cambiada exitosamente." });
                }
                else
                {
                    return BadRequest(new { Mensaje = "No se pudo cambiar la contraseña. Verifique el ID de usuario." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Mensaje = $"Error interno del servidor: {ex.Message}" });
            }

        }


        [HttpPost("VerifiqueContrasena")]
        public IActionResult VerifiqueContrasena([FromBody] SistemaDeOrdenesDeReparacion.Model.Request.VerifiqueContrasenaRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Nombre) || string.IsNullOrWhiteSpace(request.Contrasena))
            {
                return BadRequest(new { Mensaje = "Nombre de usuario y contraseña son obligatorios." });
            }
            try
            {
                var exito = gestionLogin.VerifiqueLaContrasena(request.Nombre, request.Contrasena);
                if (exito)
                {
                    return Ok(new { Mensaje = "Contraseña verificada exitosamente." });
                }
                else
                {
                    return BadRequest(new { Mensaje = "Contraseña incorrecta." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Mensaje = $"Error interno del servidor: {ex.Message}" });
            }
        }

        [HttpGet("ClientePorIdentificacionUsuario/{identificacion}")]
        public IActionResult ClientePorIdentificacionUsuario(string identificacion)
        {
            try
            {
                var cliente = gestionLogin.ObtengaClientePorIdentificacionDelUsuario(identificacion);
                if (cliente == null)
                {
                    return NotFound(new { Mensaje = "Cliente no encontrado." });
                }
                return Ok(cliente);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Mensaje = $"Error interno del servidor: {ex.Message}" });
            }
        }

    }
}
    