using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace SistemaDeOrdenesDeReparacion.BL
{
    /// <summary>
    /// Envía correos por SMTP. Las credenciales se leen de la sección "Smtp"
    /// de la configuración (appsettings, user-secrets o variables de entorno),
    /// nunca del código fuente.
    /// </summary>
    public class ServicioDeCorreo : IServicioDeCorreo
    {
        private readonly IConfiguration _configuracion;

        public ServicioDeCorreo(IConfiguration configuracion)
        {
            _configuracion = configuracion;
        }

        public void Envie(string destinatario, string asunto, string mensaje)
        {
            string servidor = _configuracion["Smtp:Servidor"] ?? "smtp.gmail.com";
            int puerto = int.TryParse(_configuracion["Smtp:Puerto"], out var p) ? p : 587;
            string usuario = _configuracion["Smtp:Usuario"]
                ?? throw new InvalidOperationException("Falta configurar Smtp:Usuario.");
            string clave = _configuracion["Smtp:Clave"]
                ?? throw new InvalidOperationException("Falta configurar Smtp:Clave.");

            using var smtpClient = new SmtpClient(servidor, puerto)
            {
                Credentials = new NetworkCredential(usuario, clave),
                EnableSsl = true,
            };

            using var mensajeCorreo = new MailMessage
            {
                From = new MailAddress(usuario),
                Subject = asunto,
                Body = mensaje,
                IsBodyHtml = false
            };

            mensajeCorreo.To.Add(destinatario);
            smtpClient.Send(mensajeCorreo);
        }
    }
}
