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

        public UsuarioController()
        {
            _admin = new UsuarioAdmin();
        }
        
        [HttpPost]
        public Usuario BuscarUsuarioPorEmail(string email)
        {
            
            return _admin.ReturnUsuarioByEmail(email);
        }

        [HttpGet]
        public List<Usuario> ObtenerUsuarios()
        {
            List<Usuario> usuarios = new List<Usuario>();
            usuarios = _admin.ReturnObtenerUsuarios();
            return usuarios;
        }



    }
}
