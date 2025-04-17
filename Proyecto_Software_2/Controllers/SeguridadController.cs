using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AppLogic.SeguridadAdmin;
using AppLogic.UsuarioAdmin;
using DTO.SeguridadDTO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon;
using Microsoft.Extensions.Options;
using System.Web;

namespace Proyecto_Software_2.Controllers
{
    [EnableCors("MyPolicy")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SeguridadController : ControllerBase
    {
        private readonly SeguridadAdmin _otpAdmin;
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;
        private readonly UsuarioAdmin _usuarioAdmin = new UsuarioAdmin();

        public SeguridadController(SeguridadAdmin seguridadAdmin, IAmazonS3 s3Client, IConfiguration config)
        {
            _s3Client = s3Client;
            _bucketName = config["AWS:BucketName"];
            _otpAdmin = seguridadAdmin;
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
                string Otp = await _otpAdmin.GenerateOTP(email);

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

            bool isValid = _otpAdmin.Verify(req.Email, req.OtpCode);

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

        [HttpPost]
        public IActionResult GeneratePresignedUrl([FromBody] PresignRequest request)
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
                var objectKey = $"uploads/{Guid.NewGuid()}{Path.GetExtension(request.FileName)}";

                var presignedUrl = _s3Client.GetPreSignedURL(new GetPreSignedUrlRequest
                {
                    BucketName = _bucketName,
                    Key = objectKey,
                    Verb = HttpVerb.PUT,
                    Expires = DateTime.UtcNow.AddMinutes(30),
                });

                return Ok(new
                {
                    uploadUrl = presignedUrl,
                    publicUrl = $"https://{_bucketName}.s3.amazonaws.com/{objectKey}"
                });
            }

            return BadRequest("Invalid file type");
        }

        private bool IsValidFileType(string contentType)
        {
            var allowedTypes = new[] { "application/pdf", "image/jpeg" };
            return allowedTypes.Contains(contentType);
        }

    }
}