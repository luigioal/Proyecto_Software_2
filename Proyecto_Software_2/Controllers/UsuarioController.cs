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

        [HttpGet]
        public Usuario ObtenerUsuario(int idUsuario)
        {
            return _admin.ReturnUsuarioById(idUsuario);
        }

        [HttpPost]
        public IActionResult CrearUsuario([FromBody] Usuario usuario)
        {
            try
            {
                _admin.CreateUsuario(usuario);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public IActionResult ModificarUsuario([FromBody] Usuario usuario)
        {
            try
            {
                _admin.UpdateUsuario(usuario);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        public IActionResult EliminarUsuario(int idUsuario)
        {
            try
            {
                _admin.DeleteUsuario(idUsuario);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



    }
}
