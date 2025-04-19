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
        private SeguridadAdmin _seguridadAdmin;
        private Notificador _notificador;

        public TransaccionController(SeguridadAdmin seguridadAdmin, Notificador notificador)
        {
            _admin = new TransaccionAdmin();
            _seguridadAdmin = seguridadAdmin;
            _notificador = notificador;
        }


        [HttpPost]
        public async Task<IActionResult> IniciarRetiro([FromBody] RetiroRequestDTO request)
        {
            try
            {
                // Validar saldo suficiente
                var usuarioAdmin = new UsuarioAdmin();
                var usuario = usuarioAdmin.ReturnUsuarioById(request.IdUsuario);

                if (usuario.Saldo < request.Monto)
                {
                    return BadRequest("Saldo insuficiente para realizar esta operación");
                }

                // Generar OTP
                string otp = await _seguridadAdmin.GenerateOTP(usuario.CorreoElectronico);

                // Retornar éxito - el frontend mostrará el formulario para ingresar OTP
                return Ok(new { Message = "Se ha enviado un código de verificación a su correo electrónico" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmarRetiro([FromBody] ConfirmacionRetiroDTO request)
        {
            try
            {
                // Obtener datos del usuario
                var usuarioAdmin = new UsuarioAdmin();
                var usuario = usuarioAdmin.ReturnUsuarioById(request.IdUsuario);

                // Verificar OTP
                bool esValido = _seguridadAdmin.Verify(usuario.CorreoElectronico, request.OTP);

                if (!esValido)
                {
                    return BadRequest("Código OTP inválido o expirado");
                }

                // Procesar retiro
                bool resultado = _admin.ProcesarRetiro(usuario.Id, request.Monto);

                if (!resultado)
                {
                    return BadRequest("No se pudo procesar el retiro");
                }

                // Enviar notificación
                await _notificador.EnviarNotificacionRetiro(usuario.CorreoElectronico, request.Monto);

                return Ok(new { Message = "Retiro procesado exitosamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

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


    }
}

