using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DTO.TransaccionDTO;
using AppLogic.TransaccionAdmin;
using AppLogic.UsuarioAdmin;
using DTO.UsuarioDTO;
using Microsoft.AspNetCore.Cors;

namespace Proyecto_Software_2.Controllers
{
    [EnableCors("MyPolicy")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TransaccionController : ControllerBase
    {
        private TransaccionAdmin _admin;

        public TransaccionController()
        {
            _admin = new TransaccionAdmin();
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

