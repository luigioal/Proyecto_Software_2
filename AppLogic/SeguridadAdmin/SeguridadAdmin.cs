using Amazon.Runtime.Internal.Auth;
using Amazon.S3;
using AppLogic.ConnectorsAdmin;
using Azure.Core;
using DTO.SeguridadDTO;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace AppLogic.SeguridadAdmin
{
    public class SeguridadAdministrador
    {
        private readonly ConcurrentDictionary<string, (string Otp, DateTime Expiry)> _otpStore = new();
        private readonly Notificador _notificador;
        private readonly AwsConnector _awsConnector;

        public SeguridadAdministrador(Notificador notificador, AwsConnector awsConnector)
        {
            _notificador = notificador;
            _awsConnector = awsConnector;
        }

        public async Task<string> GenerateOTP(string email)
        {
            // Generar código OTP de 6 dígitos
            var otp = new Random().Next(100000, 999999).ToString();

            // Guardar en el almacén con expiración de 1 minuto
            _otpStore[email] = (otp, DateTime.UtcNow.AddMinutes(1));

            // Enviar OTP por correo
            await _notificador.EnviarCodigoOtpAsync(email, otp);

            return otp; // En producción, no devolver el OTP
        }

        public bool Verify(string email, string otp)
        {
            // Verificar si existe un OTP para este email
            if (!_otpStore.TryGetValue(email, out var entry))
            {
                return false;
            }

            // Verificar si el OTP ha expirado
            if (entry.Expiry < DateTime.UtcNow)
            {
                _otpStore.TryRemove(email, out _);
                return false;
            }

            // Verificar si el OTP es correcto
            var isValid = entry.Otp == otp;

            // Si es válido, eliminar el OTP (uso único)
            if (isValid)
            {
                _otpStore.TryRemove(email, out _);
            }

            return isValid;
        }

        public PresignedUrlResponse GetPresignedURL(PresignRequest request)
        {
            var extension = Path.GetExtension(request.FileName).ToLower();
            var contentType = extension switch
            {
                ".pdf" => "application/pdf",
                ".jpg" or ".jpeg" => "image/jpeg",
                _ => "application/octet-stream" // Default
            };
            // Validate file type/size
            if (IsValidFileType(contentType))
            {
                var presignedUrl = _awsConnector.GeneratePresignedUrl(request.FileName);

                return presignedUrl;
            }
            else
            {
                return new PresignedUrlResponse() { presignedUrl = null, publicUrl = null};
            }
        }

        private bool IsValidFileType(string contentType)
        {
            var allowedTypes = new[] { "application/pdf", "image/jpeg" };
            return allowedTypes.Contains(contentType);
        }
    }
}