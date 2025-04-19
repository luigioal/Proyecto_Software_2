using DTO.SeguridadDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AppLogic.SeguridadAdmin
{
    public class Notificador
    {
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _fromEmail;
        private readonly string _username;
        private readonly string _password;
        private readonly bool _useSsl;

        public Notificador(
            string smtpServer = "smtp.gmail.com",
            int smtpPort = 587,
            string fromEmail = "noreply@yourdomain.com",
            string username = "",
            string password = "",
            bool useSsl = true)
        {
            _smtpServer = smtpServer;
            _smtpPort = smtpPort;
            _fromEmail = fromEmail;
            _username = username;
            _password = password;
            _useSsl = useSsl;
        }

        public async Task<Notificacion> EnviarNotificacionAsync(Notificacion notificacion)
        {
            try
            {
                using (var mensaje = new MailMessage())
                {
                    mensaje.From = new MailAddress(_fromEmail);
                    mensaje.To.Add(new MailAddress(notificacion.Destinatario));
                    mensaje.Subject = notificacion.Asunto;
                    mensaje.Body = notificacion.Cuerpo;
                    mensaje.IsBodyHtml = notificacion.EsHtml;

                    using (var cliente = new SmtpClient(_smtpServer, _smtpPort))
                    {
                        cliente.EnableSsl = _useSsl;
                        cliente.Credentials = new NetworkCredential(_username, _password);

                        await cliente.SendMailAsync(mensaje);
                    }
                }

                notificacion.FechaEnvio = DateTime.Now;
                notificacion.EnvioExitoso = true;
                return notificacion;
            }
            catch (Exception ex)
            {
                notificacion.EnvioExitoso = false;
                notificacion.MensajeError = ex.Message;
                Console.WriteLine($"Error al enviar correo: {ex.Message}");
                return notificacion;
            }
        }

        // Método para enviar OTP 
        public async Task<Notificacion> EnviarCodigoOtpAsync(string email, string codigoOtp)
        {
            Notificacion notificacion = new Notificacion
            {
                Destinatario = email,
                Asunto = "Código de verificación para acceso",
                Cuerpo = $@"
                <html>
                <body style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                    <div style='padding: 20px; background-color: #f7f7f7; border-radius: 5px;'>
                        <h2 style='color: #333;'>Código de verificación</h2>
                        <p>Utilice el siguiente código para completar su inicio de sesión:</p>
                        <div style='font-size: 24px; font-weight: bold; background-color: #e9e9e9; padding: 15px; text-align: center; margin: 20px 0; letter-spacing: 5px;'>
                            {codigoOtp}
                        </div>
                        <p>Este código expirará en 5 minutos.</p>
                        <p>Si usted no solicitó este código, puede ignorar este correo.</p>
                    </div>
                </body>
                </html>",
                EsHtml = true,
                Tipo = "Email"
            };

            return await EnviarNotificacionAsync(notificacion);
        }



        // Mantener el método antiguo para compatibilidad
        public async Task EnviarCorreoAsync(string destinatario, string asunto, string cuerpo, bool esHtml = true)
        {
            Notificacion notificacion = new Notificacion
            {
                Destinatario = destinatario,
                Asunto = asunto,
                Cuerpo = cuerpo,
                EsHtml = esHtml,
                Tipo = "Email"
            };

            await EnviarNotificacionAsync(notificacion);
        }

        // Añadir estos métodos a la clase Notificador existente

        public async Task EnviarNotificacionDeposito(string email, double monto)
        {
            string asunto = "Confirmación de depósito";
            string cuerpo = $@"
    <html>
    <body style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
        <div style='padding: 20px; background-color: #f7f7f7; border-radius: 5px;'>
            <h2 style='color: #333;'>Depósito Confirmado</h2>
            <p>Se ha realizado un depósito en su cuenta por el monto de: <strong>${monto}</strong></p>
            <p>Fecha: {DateTime.Now.ToString("dd/MM/yyyy HH:mm")}</p>
            <p>Si usted no realizó esta operación, por favor contacte a soporte inmediatamente.</p>
        </div>
    </body>
    </html>";

            await EnviarCorreoAsync(email, asunto, cuerpo);
        }

        public async Task EnviarNotificacionRetiro(string email, double monto)
        {
            string asunto = "Confirmación de retiro";
            string cuerpo = $@"
    <html>
    <body style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
        <div style='padding: 20px; background-color: #f7f7f7; border-radius: 5px;'>
            <h2 style='color: #333;'>Retiro Confirmado</h2>
            <p>Se ha realizado un retiro de su cuenta por el monto de: <strong>${monto}</strong></p>
            <p>Fecha: {DateTime.Now.ToString("dd/MM/yyyy HH:mm")}</p>
            <p>Si usted no autorizó esta operación, por favor contacte a soporte inmediatamente.</p>
        </div>
    </body>
    </html>";

            await EnviarCorreoAsync(email, asunto, cuerpo);
        }

        public async Task EnviarNotificacionTransaccion(string email, string tipoTransaccion, double monto, string detalles)
        {
            string asunto = $"Confirmación de {tipoTransaccion}";
            string cuerpo = $@"
    <html>
    <body style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
        <div style='padding: 20px; background-color: #f7f7f7; border-radius: 5px;'>
            <h2 style='color: #333;'>{tipoTransaccion} Realizada</h2>
            <p>Se ha realizado una operación en su cuenta:</p>
            <p><strong>Tipo:</strong> {tipoTransaccion}</p>
            <p><strong>Monto:</strong> ${monto}</p>
            <p><strong>Detalles:</strong> {detalles}</p>
            <p><strong>Fecha:</strong> {DateTime.Now.ToString("dd/MM/yyyy HH:mm")}</p>
            <p>Si usted no realizó esta operación, contacte a soporte inmediatamente.</p>
        </div>
    </body>
    </html>";

            await EnviarCorreoAsync(email, asunto, cuerpo);
        }

    }
}
