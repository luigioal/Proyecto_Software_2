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
            
            UsuarioAdmin admin = new UsuarioAdmin();
            return admin.ReturnUsuarioByEmail(email);
            
        }

        [HttpGet]
        public List<Usuario> ObtenerUsuarios()
        {
            return _admin.ReturnUsuarios();
        }

        [HttpGet]
        public List<Usuario> ObtenerAsesoresPorAdmin(int idAdmin)
        {

            return _admin.ReturnAsesoresPorAdmin(idAdmin);
        }

        [HttpGet]
        public List<Usuario> ObtenerClientesPorAsesor(int idAsesor)
        {
            return _admin.ReturnClientesPorAsesor(idAsesor);
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
