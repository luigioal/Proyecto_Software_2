using System.Globalization;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using DTO.PayPalDTO;
using AppLogic.UsuarioAdmin;
using AppLogic.SeguridadAdmin;

namespace AppLogic.ConnectorsAdmin
{
    public class PayPalConnector
    {
        private readonly string _clientId;
        private readonly string _secret;
        private readonly bool _useSandbox;
        private readonly string _apiBaseUrl;

        public PayPalConnector(IConfiguration config)
        {
            _clientId = config["PayPalSettings:ClientId"]!;
            _secret = config["PayPalSettings:Secret"]!;
            _useSandbox = bool.Parse(config["PayPal:UseSandbox"] ?? "true");
            _apiBaseUrl = _useSandbox
                ? "https://api-m.sandbox.paypal.com"
                : "https://api-m.paypal.com";
        }

        // Método para procesar un depósito (manual, sin validar con PayPal)
        public async Task<PayPalTransactionDTO> ProcesarDepositoAsync(int usuarioId, double monto, string orderIdPayPal)
        {
            var usuarioAdmin = new UsuarioAdministrador();

            try
            {
                double saldoActual = usuarioAdmin.GetUserBalance(usuarioId);
                usuarioAdmin.UpdateUserBalance(usuarioId, saldoActual + monto);

                var usuario = usuarioAdmin.ReturnUsuarioById(usuarioId);
                var notificador = new Notificador();
                await notificador.EnviarNotificacionDeposito(usuario.CorreoElectronico, monto);

                return new PayPalTransactionDTO
                {
                    UsuarioId = usuarioId,
                    Monto = monto,
                    Estado = "Completada",
                    FechaCreacion = DateTime.UtcNow,
                    PayPalTransactionId = orderIdPayPal,
                    Descripcion = "Depósito procesado exitosamente"
                };
            }
            catch (Exception ex)
            {
                return new PayPalTransactionDTO
                {
                    UsuarioId = usuarioId,
                    Monto = monto,
                    Estado = "Fallida",
                    FechaCreacion = DateTime.UtcNow,
                    PayPalTransactionId = orderIdPayPal,
                    Descripcion = $"Error: {ex.Message}"
                };
            }
        }

        // Método para procesar un retiro (usando PayPal Payouts)
        public async Task<PayPalTransactionDTO> ProcesarRetiroAsync(ConfirmarRetiroRequest request, string correoDestino)
        {
            var usuarioAdmin = new UsuarioAdministrador();
            var usuario = usuarioAdmin.ReturnUsuarioById(request.UsuarioId);
            var notificador = new Notificador();
            var seguridad = new SeguridadAdministrador(notificador);

            double saldo = usuarioAdmin.GetUserBalance(request.UsuarioId);

            if (saldo < request.Monto)
            {
                return new PayPalTransactionDTO
                {
                    UsuarioId = request.UsuarioId,
                    Monto = request.Monto,
                    Estado = "Fallida",
                    FechaCreacion = DateTime.UtcNow,
                    PayPalTransactionId = null,
                    Descripcion = "Saldo insuficiente"
                };
            }

            if (!seguridad.Verify(usuario.CorreoElectronico, request.OTP))
            {
                return new PayPalTransactionDTO
                {
                    UsuarioId = request.UsuarioId,
                    Monto = request.Monto,
                    Estado = "Fallida",
                    FechaCreacion = DateTime.UtcNow,
                    PayPalTransactionId = null,
                    Descripcion = "Código OTP inválido"
                };
            }

            try
            {
                var accessToken = await ObtenerAccessTokenAsync();

                using var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var payoutBody = new
                {
                    sender_batch_header = new
                    {
                        sender_batch_id = Guid.NewGuid().ToString(),
                        email_subject = "Has recibido un pago",
                        email_message = "Tu retiro fue procesado exitosamente."
                    },
                    items = new[]
                    {
                        new
                        {
                            recipient_type = "EMAIL",
                            amount = new
                            {
                                value = request.Monto.ToString("F2", CultureInfo.InvariantCulture),
                                currency = "USD"
                            },
                            receiver = correoDestino,
                            note = "Retiro desde plataforma StockFlow",
                            sender_item_id = Guid.NewGuid().ToString()
                        }
                    }
                };

                var json = JsonSerializer.Serialize(payoutBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync($"{_apiBaseUrl}/v1/payments/payouts", content);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new PayPalTransactionDTO
                    {
                        UsuarioId = request.UsuarioId,
                        Monto = request.Monto,
                        Estado = "Fallida",
                        FechaCreacion = DateTime.UtcNow,
                        PayPalTransactionId = null,
                        Descripcion = $"Error de PayPal: {response.StatusCode} - {responseBody}"
                    };
                }

                var jsonDoc = JsonDocument.Parse(responseBody);
                var batchId = jsonDoc.RootElement.GetProperty("batch_header").GetProperty("payout_batch_id").GetString();

                usuarioAdmin.UpdateUserBalance(request.UsuarioId, saldo - request.Monto);
                await notificador.EnviarNotificacionRetiro(usuario.CorreoElectronico, request.Monto);

                return new PayPalTransactionDTO
                {
                    UsuarioId = request.UsuarioId,
                    Monto = request.Monto,
                    Estado = "Completada",
                    FechaCreacion = DateTime.UtcNow,
                    PayPalTransactionId = batchId,
                    Descripcion = "Retiro procesado exitosamente"
                };
            }
            catch (Exception ex)
            {
                return new PayPalTransactionDTO
                {
                    UsuarioId = request.UsuarioId,
                    Monto = request.Monto,
                    Estado = "Fallida",
                    FechaCreacion = DateTime.UtcNow,
                    PayPalTransactionId = null,
                    Descripcion = $"Error: {ex.Message}"
                };
            }
        }

        internal async Task<bool> VerificarDepositoAsync(string transactionId)
        {
            throw new NotImplementedException();
        }

        // Obtener token de acceso OAuth de PayPal
        private async Task<string> ObtenerAccessTokenAsync()
        {
            var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_clientId}:{_secret}"));

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);

            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials")
            });

            var response = await client.PostAsync($"{_apiBaseUrl}/v1/oauth2/token", formContent);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error al obtener token de PayPal: {response.StatusCode} - {responseBody}");
            }

            var jsonDoc = JsonDocument.Parse(responseBody);
            return jsonDoc.RootElement.GetProperty("access_token").GetString();
        }
    }
}
