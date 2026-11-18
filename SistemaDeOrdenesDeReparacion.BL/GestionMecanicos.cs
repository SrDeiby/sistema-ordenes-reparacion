using Microsoft.EntityFrameworkCore;
    using SistemaDeOrdenesDeReparacion.DA;
    using SistemaDeOrdenesDeReparacion.Model;
    using System.Net;
    using System.Net.Mail;

    namespace SistemaDeOrdenesDeReparacion.BL
    {
        public class GestionMecanicos : IGestionMecanicos
        {
            private SistemaDeOrdenesDeReparacion.DA.DBContexto elContextoDeLaBaseDeDatos;
            private readonly IServicioDeCorreo servicioDeCorreo;


            public GestionMecanicos(SistemaDeOrdenesDeReparacion.DA.DBContexto contexto, IServicioDeCorreo correo)
            {
                elContextoDeLaBaseDeDatos = contexto;
                servicioDeCorreo = correo;
            }


            public List<Model.Mecanico> obtengaMecanicos()
            {
                return elContextoDeLaBaseDeDatos.Mecanicos.ToList();
            }
            public List<Model.Mecanico> FiltrarMecanicosPorNombre(string nombre)
            {
                var resultado = from c in elContextoDeLaBaseDeDatos.Mecanicos
                                where c.Nombre.Contains(nombre)
                                select c;
                return resultado.ToList();
            }

            public void EditeElMecanico(int id, string identificacion, string nombre, string apellidos, string correoelectronico)
            {
           
            var otro = elContextoDeLaBaseDeDatos.Mecanicos
                .FirstOrDefault(m => m.Identificacion == identificacion && m.Id != id);
            if (otro != null)
                throw new ArgumentException("Ya existe un Usuario con la identificación proporcionada");

            
            var mecanico = elContextoDeLaBaseDeDatos.Mecanicos.Find(id);
            if (mecanico == null)
                throw new ArgumentException("Mecánico no encontrado.");

            
            mecanico.Identificacion = identificacion;
            mecanico.Nombre = nombre;
            mecanico.Apellidos = apellidos;
            mecanico.CorreoElectronico = correoelectronico;

            elContextoDeLaBaseDeDatos.SaveChanges();
        }
      

            private static Random random = new Random();

            public void envieCorreoElectronico(string correoElectronico, string asunto, string mensaje)
            {
                try
                {
                    servicioDeCorreo.Envie(correoElectronico, asunto, mensaje);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error al enviar el correo electrónico: {ex.Message}", ex);
                }
            }

            public void RegistreMecanicoConCredenciales(Mecanico mecanico)
            {
                if (mecanico == null || string.IsNullOrWhiteSpace(mecanico.CorreoElectronico))
                {
                    throw new ArgumentException("El mecánico debe ser válido y tener un correo electrónico.");
                }

                string identificacion = mecanico.Identificacion;

            if (ExisteCedulaUsuario(identificacion))
            {
                throw new ArgumentException("Ya existe un Usuario con la identificación proporcionada");
            }

            string nombre = mecanico.Nombre;
            string apellidos = mecanico.Apellidos;

            if(ExisteElNombreDelUsuario(nombre, apellidos))
            {
                throw new ArgumentException("Ya existe un Usuario con el Nombre proporcionado");
            }
            else
            {
                elContextoDeLaBaseDeDatos.Mecanicos.Add(mecanico);
                elContextoDeLaBaseDeDatos.SaveChanges();


                string usuario = mecanico.Nombre.Replace(" ", "") + mecanico.Apellidos.Replace(" ", "");

                string contrasena = GenerarContrasena();

                Usuario usuarioMecanico = new Usuario();

                usuarioMecanico.Nombre = usuario;
                usuarioMecanico.Contrasena = SeguridadDeContrasenas.Hashee(contrasena);
                usuarioMecanico.Identificacion = mecanico.Identificacion;
                usuarioMecanico.Tipo = Tipo.Mecanico;
                usuarioMecanico.Estado = Estado.Activo;
                usuarioMecanico.CorreoElectronico = mecanico.CorreoElectronico;
                elContextoDeLaBaseDeDatos.Usuarios.Add(usuarioMecanico);
                elContextoDeLaBaseDeDatos.SaveChanges();



               
                string asunto = "Credenciales de acceso al sistema";
                string mensaje = $"Hola {mecanico.Nombre},\n\nTu cuenta ha sido registrada.\nUsuario: {usuario}\nContraseña: {contrasena}";

                envieCorreoElectronico(mecanico.CorreoElectronico, asunto, mensaje);

            }


                
            }

            private string GenerarContrasena(int longitud = 8)
            {
                const string caracteres = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                return new string(Enumerable.Repeat(caracteres, longitud)
                    .Select(s => s[random.Next(s.Length)]).ToArray());
            }


        private bool ExisteCedulaUsuario(string identificacion)
        {
            if (identificacion is null) return false;

            return elContextoDeLaBaseDeDatos.Usuarios.Any(u => u.Identificacion == identificacion);
        }


        private bool ExisteElNombreDelUsuario(string nombre, string apellidos)
        {
           
            var nombreUsuario = (nombre + apellidos)
                .Replace(" ", "")        
                .Trim();                   

            bool yaExiste = elContextoDeLaBaseDeDatos.Usuarios
                .Any(u => u.Nombre == nombreUsuario);

            if(yaExiste is true)
            {
                return true;
            }

            return false;
        }
        public Mecanico ObtengaMecanicoPorId(int id)
        {
            var mecanico = elContextoDeLaBaseDeDatos.Mecanicos.Find(id);
            if (mecanico == null)
            {
                throw new ArgumentException("Mecánico no encontrado.");
            }
            return mecanico;
        }

    }
    }
