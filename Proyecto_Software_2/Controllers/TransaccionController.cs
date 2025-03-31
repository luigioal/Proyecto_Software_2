using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DTO.TransaccionDTO;
using AppLogic.TransaccionAdmin;
using AppLogic.UsuarioAdmin;

namespace Proyecto_Software_2.Controllers
{
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
        public CargosExtra ObtenerCargosExtra()
        {
            return _admin.ReturnCargosExtra();
        }
    }
}
