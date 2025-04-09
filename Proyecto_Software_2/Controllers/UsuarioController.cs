using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using DTO.UsuarioDTO;
using AppLogic.UsuarioAdmin;
using DataAccess.CRUD;

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
        public IActionResult ObtenerAsesorPorCliente(int idCliente)
        {
            try
            {
                var cliente = _admin.ReturnUsuarioById(idCliente);

                if (cliente == null || cliente.Roles == null || !cliente.Roles.Contains("Cliente"))
                    return BadRequest("El usuario especificado no es un cliente válido.");

                if (!cliente.IdSupervisor.HasValue)
                    return NotFound("Este cliente no tiene un asesor asignado.");

                var asesor = _admin.ReturnUsuarioById(cliente.IdSupervisor.Value);

                if (asesor == null || asesor.Roles == null || !asesor.Roles.Contains("Asesor"))
                    return NotFound("No se encontró un asesor válido para este cliente.");

                return Ok(asesor);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener asesor: {ex.Message}");
            }
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

     [HttpPut("{email}")]
public IActionResult ModificarUsuario(string email, [FromBody] Usuario nuevosDatos)
{
    if (string.IsNullOrEmpty(email))
        return BadRequest("Se requiere un correo electrónico válido.");

    try
    {
        var usuarioExistente = _admin.ReturnUsuarioByEmail(email);
        if (usuarioExistente == null)
            return NotFound("Usuario no encontrado con el correo proporcionado.");

                // Validación del nuevo asesor (si se proporciona)
                if (nuevosDatos.IdSupervisor.HasValue)
                {
                    var asesor = _admin.ReturnUsuarioById(nuevosDatos.IdSupervisor.Value);
                    if (asesor == null || asesor.Roles == null || !asesor.Roles.Contains("Asesor"))
                    {
                        return BadRequest("El asesor especificado no existe o no tiene el rol adecuado.");
                    }

                    usuarioExistente.IdSupervisor = nuevosDatos.IdSupervisor;
                }
                

                // Reemplazar sólo los datos que vienen nuevos
        usuarioExistente.Nombre = nuevosDatos.Nombre ?? usuarioExistente.Nombre;
        usuarioExistente.PrimerApellido = nuevosDatos.PrimerApellido ?? usuarioExistente.PrimerApellido;
        usuarioExistente.SegundoApellido = nuevosDatos.SegundoApellido ?? usuarioExistente.SegundoApellido;
        usuarioExistente.Direccion = nuevosDatos.Direccion ?? usuarioExistente.Direccion;
        usuarioExistente.Contrasena = string.IsNullOrEmpty(nuevosDatos.Contrasena) ? usuarioExistente.Contrasena : nuevosDatos.Contrasena;


        _admin.UpdateUsuario(usuarioExistente);
        return Ok(new { mensaje = "Usuario actualizado correctamente" });
    }
    catch (Exception ex)
    {
        return StatusCode(500, $"Error al actualizar usuario: {ex.Message}");
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
