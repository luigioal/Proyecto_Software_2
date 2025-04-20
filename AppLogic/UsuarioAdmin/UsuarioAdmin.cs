using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.CRUD;
using DTO.UsuarioDTO;

namespace AppLogic.UsuarioAdmin
{
    public class UsuarioAdmin
    {
        private UsuarioCrud _usuarioCrud;

        public UsuarioAdmin()
        {
            _usuarioCrud = new UsuarioCrud();
        }

        public Usuario ReturnUsuarioByEmail(string email)
        {
            return _usuarioCrud.RetrieveByEmail<Usuario>(email);
        }

        public List<Usuario> ReturnUsuarios()
        {
            List<Usuario> usuarios;

            usuarios = _usuarioCrud.RetrieveAll<Usuario>();


            return usuarios;
        }

        public List<Usuario> ReturnAsesores()
        {

            List<Usuario> usuarios;

            usuarios = _usuarioCrud.RetrieveAll<Usuario>();

            List<Usuario> asesores  = new List<Usuario>();

            foreach (var user in usuarios)
            {
                if (user.Roles.Contains("Asesor"))
                {
                    asesores.Add(user);
                }
            }

            return asesores;
        }

        public List<Usuario> ReturnClientesPorAsesor(int idAsesor)
        {
            List<Usuario> clientes;

            clientes = _usuarioCrud.RetrieveAll<Usuario>(idAsesor);


            return clientes;
        }

        public Usuario ReturnUsuarioById(int idUsuario)
        {
            return _usuarioCrud.RetrieveById<Usuario>(idUsuario);
        }

        public void CreateUsuario(Usuario usuario)
        {
            _usuarioCrud.Create(usuario);

        }

        public void UpdateUsuario(Usuario usuario)
        {
            _usuarioCrud.Update(usuario);
        }

        public void UpdateRolesDeUsuario(int idUsuario, string rol)
        {
            _usuarioCrud.UpdateRol(idUsuario, rol);
        }

        public void DeleteUsuario(int id)
        {
            _usuarioCrud.Delete(id);
        }

        public void ActivateDeactivateUsuario(int idUsuario, bool nuevoEstado)
        {
            _usuarioCrud.ActivateDeactivate(idUsuario, nuevoEstado);
        }

        public bool CambiarContrasena(string email, string nuevaContrasena)
        {
            var usuario = _usuarioCrud.RetrieveByEmail<Usuario>(email);
            if (usuario == null) return false;

            usuario.Contrasena = nuevaContrasena;
            _usuarioCrud.Update(usuario);
            return true;
        }

        public int ReturnNumeroClientes(int idAsesor)
        {
            return this.ReturnClientesPorAsesor(idAsesor).Count();
        }


        public double GetUserBalance(int idUsuario)
        {
            return _usuarioCrud.RetrieveBalanceById(idUsuario);
        }

        public bool UpdateUserBalance(int idUsuario, double nuevoSaldo)
        {
            return _usuarioCrud.UpdateBalance(idUsuario, nuevoSaldo);
        }

    }
}
