using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using DTO.UsuarioDTO;
using AppLogic.UsuarioAdmin;

namespace Proyecto_Software_2.Controllers
{
    [EnableCors("MyPolicy")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private UsuarioAdmin _admin;
        
        [HttpPost]
        public Usuario GetVacationByEmail(string email)
        {
            UsuarioAdmin admin = new UsuarioAdmin();
            return admin.ReturnVacationByEmail(email);
        }

        [HttpGet]
        public List<Asesor> ObtenerAsesores()
        {
            return _admin.ObtenerAsesores();
        }
    }
}
