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
            //@Email NVARCHAR(255)
            SqlOperation operation = new SqlOperation();
            operation.procedureName = "SP_SELECT_USER_BY_EMAIL";
            operation.AddVarcharParameter("Email", email);


            return operation;
        }


        //Actualizado para manejar datos que son null y evitar errores en runtime. 
        public BaseClass MapObject(Dictionary<string, object> objectRow)
        {
            Usuario usuario = new Usuario();
            usuario.Id = Convert.ToInt32(objectRow["Id"]);
            usuario.Tipo = objectRow["Tipo"].ToString();
            usuario.Nombre = objectRow["Nombre"].ToString();
            usuario.PrimerApellido = objectRow["PrimerApellido"].ToString();
            usuario.SegundoApellido = objectRow["SegundoApellido"].ToString();

            // required DateTime 
            usuario.FechaNacimiento = DateTime.Parse(objectRow["FechaNacimiento"].ToString());

            usuario.CorreoElectronico = objectRow["CorreoElectronico"].ToString();
            usuario.Direccion = objectRow["Direccion"].ToString();
            usuario.FotoPerfil = objectRow["FotoPerfil"].ToString();
            usuario.Contrasena = objectRow["Contrasena"].ToString();
            usuario.Estado = Boolean.Parse(objectRow["Estado"].ToString());
            usuario.FechaRegistro = DateTime.Parse(objectRow["FechaRegistro"].ToString());

            // nullable DateTime 
            var ultimoAccesoValue = objectRow["UltimoAcceso"];
            if (ultimoAccesoValue != null && ultimoAccesoValue != DBNull.Value && !string.IsNullOrEmpty(ultimoAccesoValue.ToString()))
            {
                usuario.UltimoAcceso = DateTime.Parse(ultimoAccesoValue.ToString());
            }
            else
            {
                usuario.UltimoAcceso = null;
            }

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
