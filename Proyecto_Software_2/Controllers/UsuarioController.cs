using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using DTO.UsuarioDTO;
using AppLogic.UsuarioAdmin;
using DTO;

namespace Proyecto_Software_2.Controllers
{
    [EnableCors("MyPolicy")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        [HttpPost]
        public Usuario BuscarUsuarioPorEmail(string email)
        {
            
            UsuarioAdmin admin = new UsuarioAdmin();
            return admin.ReturnUsuarioByEmail(email);
        }

        [HttpPost]
        public Boolean ValidarUsuario(string email, string contrasena)
        {
            UsuarioAdmin admin = new UsuarioAdmin();
            var usuario = admin.ReturnUsuarioByEmail(email);

            if (usuario != null && usuario.Contrasena == contrasena && usuario.Estado == true)
            {
                return true;
            }

            return false;
        }
    }
}
