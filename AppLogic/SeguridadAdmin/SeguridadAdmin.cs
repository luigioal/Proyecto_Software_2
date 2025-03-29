using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace AppLogic.SeguridadAdmin
{
    public class SeguridadAdmin
    {
        private readonly ConcurrentDictionary<string, (string Otp, DateTime Expiry)> _otpStore = new();
        private readonly Notificador _notificador;

        public SeguridadAdmin(Notificador notificador)
        {
            _notificador = notificador;
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
    }
}