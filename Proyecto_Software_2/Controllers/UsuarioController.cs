using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using DTO.UsuarioDTO;
using AppLogic.UsuarioAdmin;

namespace Proyecto_Software_2.Controllers
{
    [EnableCors("AllowUI")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private UsuarioAdmin _admin;

        public UsuarioController()
        {
            _admin = new UsuarioAdmin();
        }


        [HttpGet]
        public IActionResult ObtenerBalancePorUsuarioID(int idUsuario)
        {
            try
            {
                var balance = _admin.GetUserBalance(idUsuario);
                return Ok(new { UsuarioID = idUsuario, Balance = balance });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public IActionResult ModificarBalancePorUsuarioID([FromBody] BalanceUpdate balanceUpdate)
        {
            try
            {
                bool result = _admin.UpdateUserBalance(balanceUpdate.UsuarioID, balanceUpdate.NuevoSaldo);

                if (result)
                {
                    return Ok(new { UsuarioID = balanceUpdate.UsuarioID, NuevoSaldo = balanceUpdate.NuevoSaldo, Mensaje = "Balance actualizado exitosamente" });
                }
                else
                {
                    return BadRequest("No se pudo actualizar el balance");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
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

        [HttpGet]
        public Usuario ObtenerUsuario(int idUsuario)
        {
            return _admin.ReturnUsuarioById(idUsuario);
        }

        [HttpPost]
        public IActionResult CrearUsuario([FromForm] Usuario usuario)
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
            Console.WriteLine(usuario);
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

        [HttpPut]
        public IActionResult ModificarRolesDeUsuario([FromQuery]int idUsuario, [FromQuery]string rol)
        {
            try
            {
                _admin.UpdateRolesDeUsuario(idUsuario, rol);
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

        [HttpPut]
        public IActionResult ActivarDesactivarUsuario([FromQuery]int idUsuario, [FromQuery] bool nuevoEstado)
        {
            try
            {
                _admin.ActivateDeactivateUsuario(idUsuario, nuevoEstado);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
