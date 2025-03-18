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
            //usuario.EmployeeId = int.Parse(objectRow["EmployeeId"].ToString());
            //usuario.StartDay = DateTime.Parse(objectRow["StartDay"].ToString());
            //usuario.EndDay = DateTime.Parse(objectRow["EndDay"].ToString());
            //usuario.Justification = objectRow["Justification"].ToString();
            //usuario.isActive = Boolean.Parse(objectRow["Active"].ToString());

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
