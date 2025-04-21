using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AppLogic.SeguridadAdmin;
using AppLogic.UsuarioAdmin;
using DTO.SeguridadDTO;
using Microsoft.AspNetCore.Cors;
using Amazon.S3;


namespace Proyecto_Software_2.Controllers
{
    [EnableCors("MyPolicy")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SeguridadController : ControllerBase
    {
        private readonly SeguridadAdministrador _seguridadAdmin;
        private readonly UsuarioAdministrador _usuarioAdmin;

        public SeguridadController(SeguridadAdministrador seguridadAdmin, IAmazonS3 s3Client, IConfiguration config)
        {
            _seguridadAdmin = seguridadAdmin;
            _usuarioAdmin = new UsuarioAdministrador();
        }

        [HttpPost]
        public async Task<ActionResult<Otp>> GenerarOTP(string email)
        {
            
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest(new Otp { Success = false, Message = "El correo electrónico es requerido" });
            }

            try
            {
                string Otp = await _seguridadAdmin.GenerateOTP(email);

                return Ok(new Otp
                {
                    Email = email,
                    OtpCode = Otp, 
                    ExpiresAt = DateTime.UtcNow.AddMinutes(1),
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
            if (string.IsNullOrEmpty(req.Email) || string.IsNullOrEmpty(req.OtpCode))
            {
                return BadRequest(new Otp
                {
                    Success = false,
                    Message = "El correo electrónico y el código OTP son requeridos"
                });
            }

            bool isValid = _seguridadAdmin.Verify(req.Email, req.OtpCode);

            return Ok(new Otp
            {
                Email = req.Email,
                OtpCode = req.OtpCode,
                Success = isValid,
                Message = isValid ? "Código OTP verificado exitosamente" : "Código OTP inválido o expirado"
            });
        }

        [HttpPut]
        public ActionResult<Otp> CambiarContrasena([FromBody] CambioContrasenaDTO req)
        {
            if (string.IsNullOrEmpty(req.Email) || string.IsNullOrEmpty(req.NuevaContrasena))
            {
                return BadRequest(new Otp
                {
                    Success = false,
                    Message = "Correo y nueva contraseña son requeridos"
                });
            }

            bool actualizado = _usuarioAdmin.CambiarContrasena(req.Email, req.NuevaContrasena);

            return Ok(new Otp
            {
                Email = req.Email,
                Success = actualizado,
                Message = actualizado ? "Contraseña actualizada exitosamente" : "No se pudo actualizar la contraseña"
            });
        }

        
    }
}