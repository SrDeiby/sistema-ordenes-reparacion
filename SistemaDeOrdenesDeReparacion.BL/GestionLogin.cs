using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using SistemaDeOrdenesDeReparacion.DA;
using SistemaDeOrdenesDeReparacion.Model;
using SistemaDeOrdenesDeReparacion.Model.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeOrdenesDeReparacion.BL
{
    public class GestionLogin : IGestionLogin
    {
        private readonly DA.DBContexto _dbContexto;
        private readonly IServicioDeCorreo _servicioDeCorreo;

        public GestionLogin(DA.DBContexto dbContexto, IServicioDeCorreo servicioDeCorreo)
        {
            _dbContexto = dbContexto;
            _servicioDeCorreo = servicioDeCorreo;
        }

        /// <summary>
        /// Busca un usuario por nombre y verifica su contraseña con BCrypt.
        /// Si la contraseña guardada estaba en texto plano, la reemplaza por su hash.
        /// </summary>
        private Usuario? BusqueUsuarioConCredenciales(string nombre, string contrasena, Tipo? tipo = null)
        {
            var candidatos = _dbContexto.Usuarios
                .Where(u => u.Nombre == nombre)
                .ToList();

            foreach (var usuario in candidatos)
            {
                if (tipo.HasValue && usuario.Tipo != tipo.Value)
                    continue;

                if (!SeguridadDeContrasenas.Verifique(contrasena, usuario.Contrasena))
                    continue;

                if (!SeguridadDeContrasenas.EsUnHash(usuario.Contrasena))
                    MigreContrasenaAHash(usuario, contrasena);

                return usuario;
            }

            return null;
        }

        private void MigreContrasenaAHash(Usuario usuario, string contrasena)
        {
            var contrasenaOriginal = usuario.Contrasena;
            try
            {
                usuario.Contrasena = SeguridadDeContrasenas.Hashee(contrasena);
                _dbContexto.SaveChanges();
            }
            catch (Exception ex)
            {
                // Si la columna no admite el hash (por ejemplo, es muy corta), se deja la contraseña como estaba.
                var entrada = _dbContexto.Entry(usuario);
                entrada.Property(u => u.Contrasena).CurrentValue = contrasenaOriginal;
                entrada.Property(u => u.Contrasena).IsModified = false;
                Console.WriteLine($"No se pudo migrar la contraseña del usuario {usuario.Nombre} a hash: {ex.Message}");
            }
        }

        public Usuario InicieSesionAdministrador(string nombre, string contrasena)
        {
            var administrador = BusqueUsuarioConCredenciales(nombre, contrasena, Tipo.Administrador);

            if (administrador != null)
            {
                EnvieCorreoDeInicioDeSesionAlAdministrador(administrador);
                return administrador;
            }

            return null;
        }


        public void EnvieCorreoDeInicioDeSesionAlAdministrador(Model.Usuario administrador)
        {

            DateTime horaEnCostaRica = DateTime.UtcNow.AddHours(-6); 

            string asunto = $"Inicio de Sesión del usuario {administrador.Nombre}";
            string cuerpo = $"Usted inició sesión el día {horaEnCostaRica:dd/MM/yyyy} a las {horaEnCostaRica:HH:mm}";


            try
            {
                _servicioDeCorreo.Envie(administrador.CorreoElectronico, asunto, cuerpo);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al enviar el correo: " + ex.Message);
            }
        }

        public bool InicieUnaSesionDelUsuario(string nombre, string contrasena)
        {
            
                
                var usuario = BusqueUsuarioConCredenciales(nombre, contrasena);


                if (usuario != null)
                {

                    string correoElectronico = null;

                    var cliente = _dbContexto.Clientes.FirstOrDefault(c => c.Identificacion == usuario.Identificacion);
                    if (cliente != null)
                    {

                    DateTime horaEnCostaRica = DateTime.UtcNow.AddHours(-6);
                    correoElectronico = cliente.CorreoElectronico;
                        string asunto = $"Inicio de Sesión del usuario {usuario.Nombre}";
                        string mensaje = $"Usted inició sesión el día {horaEnCostaRica:dd/MM/yyyy} a las {horaEnCostaRica:HH:mm}";
                        envieCorreoElectronicoDeSesion(correoElectronico, asunto, mensaje);
                        return true;
                    }


                    if (correoElectronico == null)
                    {
                        var mecanico = _dbContexto.Mecanicos.FirstOrDefault(m => m.Identificacion == usuario.Identificacion);
                        if (mecanico != null)
                        {
                        DateTime horaEnCostaRica = DateTime.UtcNow.AddHours(-6);
                        correoElectronico = mecanico.CorreoElectronico;
                            string asunto = $"Inicio de Sesión del usuario {usuario.Nombre}";
                            string mensaje = $"Usted inició sesión el día {horaEnCostaRica:dd/MM/yyyy}  a las  {horaEnCostaRica:HH:mm}";
                            envieCorreoElectronicoDeSesion(correoElectronico, asunto, mensaje);
                            return true;
                        }
                    }

                }
            return false;
            }
           
        

        public void envieCorreoElectronicoDeSesion(string correoElectronico, string asunto, string mensaje)
        {
            try
            {
                _servicioDeCorreo.Envie(correoElectronico, asunto, mensaje);
            }
            catch (Exception ex)
            {
                // El correo es solo una notificación: si falla, se registra y la operación continúa.
                Console.WriteLine($"No se pudo enviar el correo de notificación a {correoElectronico}: {ex.Message}");
            }
        }

        public Model.Mecanico ObtengaMecanicoPorNombreyContrasenaDeUsuario(string nombre, string contrasena)
        {
            var usuario = BusqueUsuarioConCredenciales(nombre, contrasena);
            if (usuario == null)
            {
                throw new Exception("Usuario no encontrado.");
            }
            var mecanico = _dbContexto.Mecanicos.FirstOrDefault(m => m.Identificacion == usuario.Identificacion);
            if (mecanico == null)
            {
                throw new Exception("Mecánico no encontrado.");
            }
            return mecanico;
        }

        public Model.Cliente ObtengaClientePorNombreyContrasenaDeUsuario(string nombre, string contrasena)
        {
            var usuario = BusqueUsuarioConCredenciales(nombre, contrasena);
            if (usuario == null)
            {
                throw new Exception("Usuario no encontrado.");
            }
            var cliente = _dbContexto.Clientes.FirstOrDefault(c => c.Identificacion == usuario.Identificacion);
            if (cliente == null)
            {
                throw new Exception("Cliente no encontrado.");
            }
            return cliente;
        }

        public string ObtengaIdentifiacionDelMecanicoPorNombreYContrasenaDeUsuario(string nombre, string contrasena)
        {
            var usuario = BusqueUsuarioConCredenciales(nombre, contrasena);
            if (usuario == null)
            {
                throw new Exception("Usuario no encontrado.");
            }
            var mecanico = _dbContexto.Mecanicos.FirstOrDefault(m => m.Identificacion == usuario.Identificacion);
            if (mecanico == null)
            {
                throw new Exception("Mecánico no encontrado.");
            }
            return mecanico.Identificacion;
        }

        public string ObtengaIdentificacionDelClientePorNombreYContrasenaDeUsuario(string nombre, string contrasena)
        {
            var usuario = BusqueUsuarioConCredenciales(nombre, contrasena);
            if (usuario == null)
            {
                throw new Exception("Usuario no encontrado.");
            }
            var cliente = _dbContexto.Clientes.FirstOrDefault(c => c.Identificacion == usuario.Identificacion);
            if (cliente == null)
            {
                throw new Exception("Cliente no encontrado.");
            }
            return cliente.Identificacion;
        }

        public Model.Cliente ObtengaClientePorIdentificacionDelUsuario(String identificacion)
        {
            var usuario = _dbContexto.Usuarios.FirstOrDefault(u => u.Identificacion == identificacion);
            if (usuario == null)
            {
                throw new Exception("Usuario no encontrado.");
            }
            var cliente = _dbContexto.Clientes.FirstOrDefault(c => c.Identificacion == usuario.Identificacion);
            if (cliente == null)
            {
                throw new Exception("Cliente no encontrado.");
            }
            return cliente;
        }
        public Model.Mecanico ObtengaMecanicoPorIdentificacionDelUsuario(String identificacion)
        {

            var usuario = _dbContexto.Usuarios.FirstOrDefault(u => u.Identificacion == identificacion);
            if (usuario == null)
            {
                throw new Exception("Usuario no encontrado.");
            }

            var mecanico = _dbContexto.Mecanicos.FirstOrDefault(m => m.Identificacion == usuario.Identificacion);
            if (mecanico == null)
            {
                throw new Exception("Mecánico no encontrado.");
            }
            return mecanico;
        }

        public Model.Mecanico ObtengaMecanicoPorIdDelUsuario(int id)
        {
            var usuario = _dbContexto.Usuarios.Find(id);
            if (usuario == null)
            {
                throw new Exception("Usuario no encontrado.");
            }
            var mecanico = _dbContexto.Mecanicos.FirstOrDefault(m => m.Identificacion == usuario.Identificacion);
            if (mecanico == null)
            {
                throw new Exception("Mecánico no encontrado.");
            }
            return mecanico;
        }

        public Model.Usuario ObtengaUsuarioPorIdDelMecanico(int id)
        {
            var usuario = _dbContexto.Usuarios.FirstOrDefault(u => u.Id == id);
            if (usuario == null)
            {
                throw new Exception("Usuario no encontrado.");
            }
            return usuario;
        }

        public Model.Usuario ObtengaUsuarioPorIdDelCliente(int id)
        {
            var usuario = _dbContexto.Usuarios.FirstOrDefault(u => u.Id == id);
            if (usuario == null)
            {
                throw new Exception("Usuario no encontrado.");
            }
            return usuario;
        }

        public Model.Usuario ObtengaUsuarioPorId(int id)
        {
            var usuario = _dbContexto.Usuarios.Find(id);
            if (usuario == null)
            {
                throw new Exception("Usuario no encontrado.");
            }
            return usuario;
        }

        public Model.Usuario ObtengaUsuarioPorIdentificacionDelMecanico(String identificacionMecanico)
        {
            var mecanico = _dbContexto.Mecanicos.FirstOrDefault(m => m.Identificacion == identificacionMecanico);
            if (mecanico == null)
            {
                throw new Exception("Mecánico no encontrado.");
            }

            var usuario = _dbContexto.Usuarios.FirstOrDefault(u => u.Identificacion == mecanico.Identificacion);
            if (usuario == null)
            {
                throw new Exception("Usuario no encontrado.");
            }

            return usuario;
        }

        public Model.Usuario ObtengaUsuarioPorIdentificacionDelCliente(String identificacionCliente)
        {
            var cliente = _dbContexto.Clientes.FirstOrDefault(c => c.Identificacion == identificacionCliente);
            if (cliente == null)
            {
                throw new Exception("Cliente no encontrado.");
            }
            var usuario = _dbContexto.Usuarios.FirstOrDefault(u => u.Identificacion == cliente.Identificacion);
            if (usuario == null)
            {
                throw new Exception("Usuario no encontrado.");
            }
            return usuario;
        }


        public int ObtengaIdDeUsuarioPorNombreYContrasena(string nombre, string contrasena)
        {
            var usuario = BusqueUsuarioConCredenciales(nombre, contrasena);
            if (usuario == null)
            {
                throw new Exception("Usuario no encontrado.");
            }
            return usuario.Id;
        }

        public string ObtengaIdentificacionDelMecanicoPorElIdUsuario(int id)
        {
            var usuario = _dbContexto.Usuarios.Find(id);
            if (usuario == null)
            {
                throw new Exception("Usuario no encontrado.");
            }
            var mecanico = _dbContexto.Mecanicos.FirstOrDefault(m => m.Identificacion == usuario.Identificacion);
            if (mecanico == null)
            {
                throw new Exception("Mecánico no encontrado.");
            }
            return mecanico.Identificacion.ToString();
        }

        public bool CambieLaContrasenaDelUsuario(int id, string nuevaContrasena)
        {
            String correoElectronico = null;
            String mensaje = null;
            String asunto = null;


            var usuario = _dbContexto.Usuarios.FirstOrDefault(u => u.Id == id);

            if (usuario != null)
            {

                var mecanico = _dbContexto.Mecanicos.FirstOrDefault(m => m.Identificacion == usuario.Identificacion);
                if (mecanico != null)
                {
                    var usuario_mecanico = _dbContexto.Usuarios.FirstOrDefault(u => u.Identificacion == mecanico.Identificacion);
                    correoElectronico = mecanico.CorreoElectronico;
                    mensaje = "Le informamos que la cuenta del usuario " + usuario_mecanico.Nombre + " ha cambiado su clave.";
                    asunto = "Cambio de clave del usuario " + usuario_mecanico.Nombre;
                    envieCorreoElectronicoDeSesion(correoElectronico, asunto, mensaje);
                    usuario.Contrasena = SeguridadDeContrasenas.Hashee(nuevaContrasena);
                    _dbContexto.SaveChanges();

                }
                var cliente = _dbContexto.Clientes.FirstOrDefault(c => c.Identificacion == usuario.Identificacion);
                if (cliente != null)
                {
                    var usuario_cliente = _dbContexto.Usuarios.FirstOrDefault(u => u.Identificacion == cliente.Identificacion);

                    correoElectronico = cliente.CorreoElectronico;
                    mensaje = "Le informamos que la cuenta del usuario " + usuario_cliente.Nombre + " ha cambiado su clave.";
                    asunto = "Cambio de clave del usuario " + usuario_cliente.Nombre;
                    envieCorreoElectronicoDeSesion(correoElectronico, asunto, mensaje);
                    usuario.Contrasena = SeguridadDeContrasenas.Hashee(nuevaContrasena);
                    _dbContexto.SaveChanges();
                }
                return true;
            }
            else
            {
                throw new Exception("Usuario no encontrado.");
                return false;
            }

        }

        public bool VerifiqueLaContrasena(string nombre, string contrasena)
        {

            var usuario = BusqueUsuarioConCredenciales(nombre, contrasena);

            if (usuario != null)
            {
                return true;
            }
            return false;


        }

        public bool SiElUsuarioSeBloqueaDespuesDeDosIntentos(string nombre, string contrasena)
        {
            var usuario = _dbContexto.Usuarios.FirstOrDefault(u => u.Nombre.Equals(nombre));

            if (usuario == null || usuario.Tipo == Tipo.Administrador)
                return false;

          
            if (usuario.Estado == Estado.Bloqueado)
            {

                DateTime horaEnCostaRica = DateTime.UtcNow.AddHours(-6);

                if (usuario.FechaBloqueo.HasValue && usuario.FechaBloqueo.Value.AddMinutes(3) <= horaEnCostaRica)
                {
                    usuario.Estado = Estado.Activo;
                    usuario.IntentosFallidos = 0;
                    usuario.FechaBloqueo = null;
                    _dbContexto.SaveChanges();
                    return false;
                }
                else
                {
                    string asunto = $"Intento de inicio de sesión del usuario {usuario.Nombre} bloqueado";
                    string mensaje =
                        $"Le informamos que la cuenta del usuario {usuario.Nombre} " +
                        $"se encuentra bloqueada por 3 minutos. Podrá ingresar el día " +
                        $"{usuario.FechaBloqueo.Value.AddMinutes(3):dd/MM/yyyy HH:mm}. (Hora en la que se acaba el bloqueo)";
                    envieCorreoElectronicoDeSesion(usuario.CorreoElectronico, asunto, mensaje);
                    return true;

                }


                    
            }

            
            if (!SeguridadDeContrasenas.Verifique(contrasena, usuario.Contrasena))
            {
                if (usuario.IntentosFallidos == null)
                {
                    usuario.IntentosFallidos = 1;
                }
                else
                {
                    usuario.IntentosFallidos++;
                }

                if (usuario.IntentosFallidos >= 2)
                {

                    DateTime horaEnCostaRica = DateTime.UtcNow.AddHours(-6);
                    usuario.Estado = Estado.Bloqueado;
                    usuario.FechaBloqueo = horaEnCostaRica;
                    _dbContexto.SaveChanges();

                    
                    var mecanico = _dbContexto.Mecanicos
                        .FirstOrDefault(m => m.Identificacion == usuario.Identificacion);

                    if (mecanico != null)
                    {
                        
                        string asunto = "Usuario Bloqueado";
                        string mensaje =
                            $"Le informamos que la cuenta del usuario {usuario.Nombre} " +
                            $"se ha bloqueado por 3 minutos. Podrá intentarlo de nuevo a las " +
                            $"{usuario.FechaBloqueo.Value.AddMinutes(3):dd/MM/yyyy HH:mm}.";
                        envieCorreoElectronicoDeSesion(mecanico.CorreoElectronico, asunto, mensaje);
                        return true;
                    }

                   
                    var cliente = _dbContexto.Clientes
                        .FirstOrDefault(c => c.Identificacion == usuario.Identificacion);

                    if (cliente != null)
                    {
                        string asunto = "Usuario Bloqueado";
                        string mensaje =
                            $"Le informamos que la cuenta del usuario {usuario.Nombre} " +
                            $"se ha bloqueado por 3 minutos. Podrá intentarlo de nuevo a las " +
                            $"{usuario.FechaBloqueo.Value.AddMinutes(3):dd/MM/yyyy HH:mm}.";
                        envieCorreoElectronicoDeSesion(cliente.CorreoElectronico, asunto, mensaje);
                        return true;
                    }

                    
                    return false;
                }


                _dbContexto.SaveChanges();
                return false;
            }
            else
            {
                
                usuario.IntentosFallidos = 0;
                usuario.FechaBloqueo = null;
                _dbContexto.SaveChanges();
                return false;
            }
        }   

        public Model.Cliente ObtengaClientePorIdUsuario(int id)
        {

            var usuario = _dbContexto.Usuarios.Find(id);
            if (usuario == null)
            {
                throw new Exception("Usuario no encontrado.");
            }
            var cliente = _dbContexto.Clientes.FirstOrDefault(m => m.Identificacion == usuario.Identificacion);
            if (cliente == null)
            {
                throw new Exception("Cliente no encontrado.");
            }
            return cliente;


        }

        public LoginResponse VerificarCredenciales(string nombre, string contrasena)
        {
            var usuario = BusqueUsuarioConCredenciales(nombre, contrasena);
            if (usuario == null)
            {
                return new Model.Response.LoginResponse
                {
                    Exito = false,
                    Mensaje = "Nombre de usuario o contraseña incorrectos."
                };
            }
            if (usuario.Estado == Estado.Bloqueado)
            {
                return new Model.Response.LoginResponse
                {
                    Exito = false,
                    Mensaje = "El usuario está bloqueado. Intente más tarde."
                };
            }
            return new Model.Response.LoginResponse
            {
                Exito = true,
                Mensaje = "Inicio de sesión exitoso.",
                Rol = usuario.Tipo.ToString(),
                Identificacion = usuario.Identificacion
            };
        }

    }
}
