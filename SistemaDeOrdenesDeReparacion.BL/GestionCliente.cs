using SistemaDeOrdenesDeReparacion.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SistemaDeOrdenesDeReparacion.BL
{
    public class GestionCliente : IGestionCliente 
    {

        private SistemaDeOrdenesDeReparacion.DA.DBContexto elContextoDeLaBaseDeDatos;
        private readonly IServicioDeCorreo servicioDeCorreo;

        public GestionCliente(SistemaDeOrdenesDeReparacion.DA.DBContexto contexto, IServicioDeCorreo correo)
        {
            elContextoDeLaBaseDeDatos = contexto;
            servicioDeCorreo = correo;
        }


        public List<Model.Cliente> obtengaCliente()
        {
            return elContextoDeLaBaseDeDatos.Clientes.ToList();
        }


        public List<Model.Cliente> FiltrarClientesPorNombre(string nombre)
        {
            var resultado = from c in elContextoDeLaBaseDeDatos.Clientes
                            where c.Nombre.Contains(nombre)
                            select c;
            return resultado.ToList();
        }

        public void EditeElCliente(int id, string identificacion, string nombre, string apellidos, string correoelectronico)
        {

            var otro = elContextoDeLaBaseDeDatos.Clientes
                .FirstOrDefault(m => m.Identificacion == identificacion && m.Id != id);
            if (otro != null)
                throw new ArgumentException("Ya existe un cliente con esa cédula.");


            var cliente = elContextoDeLaBaseDeDatos.Clientes.Find(id);
            if (cliente == null)
                throw new ArgumentException("Cliente no encontrado.");


            cliente.Identificacion = identificacion;
            cliente.Nombre = nombre;
            cliente.Apellidos = apellidos;
            cliente.CorreoElectronico = correoelectronico;

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


        public void RegistreClienteConCredenciales(Cliente cliente)
        {
            if (cliente == null || string.IsNullOrWhiteSpace(cliente.CorreoElectronico))
            {
                throw new ArgumentException("El mecánico debe ser válido y tener un correo electrónico.");
            }

            string identificacion = cliente.Identificacion;
            if (ExisteCedulaUsuario(identificacion))
            {
                throw new ArgumentException("Ya existe un Usuario con la identifiación proporcionada");
            }

            string nombre = cliente.Nombre;
            string apellidos = cliente.Apellidos;

            if (ExisteElNombreDelUsuario(nombre, apellidos))
            {
                throw new ArgumentException("Ya existe un Usuario con el Nombre proporcionado");
            }
            else
            {
                elContextoDeLaBaseDeDatos.Clientes.Add(cliente);
                elContextoDeLaBaseDeDatos.SaveChanges();


                string usuario = cliente.Nombre.Replace(" ", "") + cliente.Apellidos.Replace(" ", "");

                string contrasena = GenerarContrasena();

                Usuario usuarioCliente = new Usuario();

                usuarioCliente.Nombre = usuario;
                usuarioCliente.Contrasena = SeguridadDeContrasenas.Hashee(contrasena);
                usuarioCliente.Identificacion = cliente.Identificacion;
                usuarioCliente.Tipo = Tipo.Cliente;
                usuarioCliente.Estado = Estado.Activo;
                usuarioCliente.CorreoElectronico = cliente.CorreoElectronico;
                elContextoDeLaBaseDeDatos.Usuarios.Add(usuarioCliente);
                elContextoDeLaBaseDeDatos.SaveChanges();



                
                string asunto = "Credenciales de acceso al sistema";
                string mensaje = $"Hola {cliente.Nombre},\n\nTu cuenta ha sido registrada.\nUsuario: {usuario}\nContraseña: {contrasena}";

                envieCorreoElectronico(cliente.CorreoElectronico, asunto, mensaje);

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

            if (yaExiste is true)
            {
                return true;
            }

            return false;
        }
        public Cliente ObtengaClientePorId(int id)
        {
            var cliente = elContextoDeLaBaseDeDatos.Clientes.Find(id);
            if (cliente == null)
            {
                throw new ArgumentException("Cliente no encontrado.");
            }
            return cliente;
        }
    }
    }
