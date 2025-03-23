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

        public List<Usuario> ReturnObtenerUsuarios()
        {
            List<Usuario> usuarios;

            usuarios = _usuarioCrud.RetrieveAll<Usuario>();


            return usuarios;
        }
    }
}
