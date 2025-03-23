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

        public List<Usuario> ReturnAsesoresPorAdmin(int idAdmin)
        {
            List<Usuario> asesores;

            asesores = _usuarioCrud.RetrieveAll<Usuario>();

            return asesores;
        }

        public List<Usuario> ReturnClientesPorAsesor(int idAsesor)
        {
            List<Usuario> clientes;

            clientes = _usuarioCrud.RetrieveAll<Usuario>();


            return clientes;
        }
    }
}
