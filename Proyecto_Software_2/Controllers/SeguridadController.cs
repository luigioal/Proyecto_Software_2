using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AppLogic.SeguridadAdmin;
using DTO.SeguridadDTO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;

namespace Proyecto_Software_2.Controllers
{
    [EnableCors("MyPolicy")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SeguridadController : ControllerBase
    {
        private readonly SeguridadAdmin _otpAdmin;

        public SeguridadController(SeguridadAdmin seguridadAdmin)
        {
            _otpAdmin = seguridadAdmin;
        }

        [HttpPost]
        public async Task<ActionResult<Otp>> GenerarOTP([FromBody] Otp req)
        {
            if (string.IsNullOrEmpty(req.Email))
            {
                return BadRequest(new Otp { Success = false, Message = "El correo electrónico es requerido" });
            }

            try
            {
                string otp = await _otpAdmin.GenerateOTP(req.Email);

                return Ok(new Otp
                {
                    Email = req.Email,
                    otpCode = otp, 
                    ExpiresAt = DateTime.UtcNow.AddMinutes(5),
                    Success = true,
                    Message = "Código OTP enviado exitosamente"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Otp
                {
                    Success = false,
                    Message = $"Error: {ex.Message}"
                });
            }
        }

        [HttpPost]
        public ActionResult<Otp> ValidarOTP([FromBody] Otp req)
        {
            if (string.IsNullOrEmpty(req.Email) || string.IsNullOrEmpty(req.otpCode))
            {
                return BadRequest(new Otp
                {
                    Success = false,
                    Message = "El correo electrónico y el código OTP son requeridos"
                });
            }

            bool isValid = _otpAdmin.Verify(req.Email, req.otpCode);

            return Ok(new Otp
            {
                Email = req.Email,
                Success = isValid,
                Message = isValid ? "Código OTP verificado exitosamente" : "Código OTP inválido o expirado"
            });
        }
    }
}