using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DTO.TransaccionDTO;
using AppLogic.TransaccionAdmin;
using AppLogic.UsuarioAdmin;
using DTO.UsuarioDTO;
using Microsoft.AspNetCore.Cors;
using AppLogic.SeguridadAdmin;

namespace Proyecto_Software_2.Controllers
{
    [EnableCors("MyPolicy")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TransaccionController : ControllerBase
    {
        private TransaccionAdmin _admin;
        private SeguridadAdministrador _seguridadAdmin;
        private Notificador _notificador;
        private readonly UsuarioAdministrador _usuarioAdmin;

        public TransaccionController(TransaccionAdmin transaccionAdmin, SeguridadAdministrador seguridadAdmin, Notificador notificador, UsuarioAdministrador usuarioAdmin)
        {
            _admin = transaccionAdmin;
            _seguridadAdmin = seguridadAdmin;
            _notificador = notificador;
            _usuarioAdmin = usuarioAdmin;
        }


        #region Endpoints para Depósitos 
        [HttpPost("deposito/paypal/iniciar")]
        public async Task<IActionResult> IniciarDepositoPayPal([FromBody] DepositoPayPalRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var usuario = _usuarioAdmin.ReturnUsuarioById(request.UsuarioId); // Ahora usa la dependencia inyectada
                if (usuario == null)
                    return NotFound("Usuario no encontrado");

                // Resto del método igual...
                var otp = await _seguridadAdmin.GenerateOTP(usuario.CorreoElectronico);

                return Ok(new
                {
                    Message = "Se ha enviado un OTP a su correo",
                    NextStep = "confirmar_otp"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        [HttpPost("deposito/paypal/confirmar")]
        public async Task<IActionResult> ConfirmarDepositoPayPal([FromBody] ConfirmarDepositoRequest request)
        {
            try
            {
                // 1. Validar OTP
                var usuario = _usuarioAdmin.ReturnUsuarioById(request.UsuarioId);
                if (!_seguridadAdmin.Verify(usuario.CorreoElectronico, request.OTP))
                    return BadRequest("OTP inválido o expirado");

                // 2. Procesar depósito con PayPal
                var resultado = await _admin.ProcesarDepositoPayPal(
                    request.UsuarioId,
                    request.Monto,
                    request.PayPalTransactionId);

                if (!resultado.Exito)
                    return BadRequest(resultado.Mensaje);

                return Ok(new
                {
                    resultado.Exito,
                    resultado.Mensaje,
                    NuevoSaldo = usuario.Saldo
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }
        #endregion

        #region Endpoints para Retiros (RF20/RF21)
        [HttpPost("retiro/solicitar")]
        public async Task<IActionResult> SolicitarRetiro([FromBody] SolicitudRetiroRequest request)
        {
            try
            {
                // 1. Validar saldo suficiente (RF20)
                if (!_admin.ValidarSaldoSuficiente(request.UsuarioId, request.Monto))
                    return BadRequest("Saldo insuficiente para esta transacción");

                // 2. Generar y enviar OTP (RF21)
                var usuario = _usuarioAdmin.ReturnUsuarioById(request.UsuarioId);
                var otp = await _seguridadAdmin.GenerateOTP(usuario.CorreoElectronico);

                // 3. Guardar solicitud temporal 
                // _transaccionAdmin.RegistrarSolicitudRetiro(...);

                return Ok(new
                {
                    Message = "Se ha enviado un OTP a su correo electrónico",
                    NextStep = "confirmar_retiro"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        [HttpPost("retiro/confirmar")]
        public async Task<IActionResult> ConfirmarRetiro([FromBody] ConfirmarRetiroRequest request)
        {
            try
            {
                // 1. Validar OTP
                var usuario = _usuarioAdmin.ReturnUsuarioById(request.UsuarioId);
                if (!_seguridadAdmin.Verify(usuario.CorreoElectronico, request.OTP))
                    return BadRequest("OTP inválido o expirado");

                // 2. Procesar retiro
                var resultado = await _admin.ProcesarRetiroConOTP(
                    request.UsuarioId,
                    request.Monto,
                    request.OTP);

                if (!resultado.Exito)
                    return BadRequest(resultado.Mensaje);

                return Ok(new
                {
                    resultado.Exito,
                    resultado.Mensaje,
                    NuevoSaldo = usuario.Saldo
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }
        #endregion


        [HttpGet]
        public IActionResult ObtenerCargosExtra()
        {
            try
            {
                CargosExtra cargosExtra = _admin.ReturnCargosExtra();
                return Ok(cargosExtra);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult ModificarCargosExtra(CargosExtra cargosExtra)
        {
            try
            {
                _admin.ModifyCargoExtra(cargosExtra);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public IActionResult ObtenerInversiones([FromQuery] int idCliente,
                                                [FromQuery] string tipo, // Compra, Venta   
                                                [FromQuery] DateTime fechaInicio,
                                                [FromQuery] DateTime fechaFin)
        {
            try
            {
                fechaInicio = new DateTime(2020, 01, 01);
                fechaFin = DateTime.Now;
                List<InversionCard> inversiones = _admin.ReturnInversiones(idCliente, tipo, fechaInicio, fechaFin);
                return Ok(inversiones);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult VenderInversion([FromBody] Inversion inversion)
        {
            try
            {
                _admin.DoVenderInversion(inversion);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

