using DataAccess.DAO;
using DTO;
using DTO.UsuarioDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.MAPPERS
{
    public class UsuarioMapper : ICrudQueries, IObjectMapper
    {
        

       

        public SqlOperation GetRetrieveByEmailQuery(string email)
        {
            SqlOperation operation = new SqlOperation();
            operation.procedureName = "SP_SELECT_USER_BY_EMAIL";
            operation.AddVarcharParameter("Email", email);


            return operation;
        }

        

        public BaseClass MapObject(Dictionary<string, object> objectRow)
        {
            Usuario usuario = new Usuario();
            usuario.Id = int.Parse(objectRow["Id"].ToString());
            usuario.Nombre = objectRow["Nombre"].ToString();
            usuario.PrimerApellido = objectRow["PrimerApellido"].ToString();
            usuario.PrimerApellido = objectRow["SegundoApellido"].ToString();
            usuario.FechaNacimiento = DateTime.Parse(objectRow["FechaNacimiento"].ToString());
            usuario.CorreoElectronico = objectRow["CorreoElectronico"].ToString();
            usuario.Direccion = objectRow["Direccion"].ToString();
            usuario.FotoPerfil = objectRow["FotoPerfil"].ToString();
            usuario.Contrasena = objectRow["Contrasena"].ToString();
            usuario.Estado = Boolean.Parse(objectRow["Estado"].ToString());
            usuario.FechaRegistro = DateTime.Parse(objectRow["FechaRegistro"].ToString());
            usuario.UltimoAcceso = DateTime.Parse(objectRow["UltimoAcceso"].ToString());
            

            return usuario;

        }

        public List<BaseClass> MapObjectList(List<Dictionary<string, object>> objectList)
        {
            var list = new List<BaseClass>();

            foreach (var objectRow in objectList)
            {
                {
                    var usuario = MapObject(objectRow);
                    list.Add(usuario);
                }


            }
            return list;
        }
    }
}
