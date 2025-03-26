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
        public SqlOperation GetRetrieveAllQuery()
        {
            SqlOperation operation = new SqlOperation();
            operation.procedureName = "SP_SELECT_ALL_USERS";

            return operation;
        }

        public SqlOperation GetRetrieveAllQuery(int idSuper)
        {
            SqlOperation operation = new SqlOperation();
            operation.procedureName = "SP_SELECT_ALL_USERS_BY_SUPER";//Deberia devolver los usuarios con un supervisor asociado(Admin o Asesor) suministrado
            operation.AddIntegerParameter("IdSuper", idSuper);

            return operation;
        }

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

            
            int? GetInt(string key)
            {
                if (!objectRow.ContainsKey(key) || objectRow[key] == null || objectRow[key] == DBNull.Value)
                    return null;

                try { return Convert.ToInt32(objectRow[key]); }
                catch { return null; }
            }

            double? GetDouble(string key)
            {
                if (!objectRow.ContainsKey(key) || objectRow[key] == null || objectRow[key] == DBNull.Value)
                    return null;

                try { return Convert.ToDouble(objectRow[key]); }
                catch { return null; }
            }

            bool? GetBool(string key)
            {
                if (!objectRow.ContainsKey(key) || objectRow[key] == null || objectRow[key] == DBNull.Value)
                    return null;

                try { return Convert.ToBoolean(objectRow[key]); }
                catch { return null; }
            }

            DateTime? GetDateTime(string key)
            {
                if (!objectRow.ContainsKey(key) || objectRow[key] == null || objectRow[key] == DBNull.Value)
                    return null;

                try { return DateTime.Parse(objectRow[key].ToString()); }
                catch { return null; }
            }

            string? GetString(string key)
            {
                if (!objectRow.ContainsKey(key) || objectRow[key] == null || objectRow[key] == DBNull.Value)
                    return null;

                return objectRow[key].ToString();
            }

            
            usuario.Id = GetInt("UsuarioID") ?? 0; // BaseClass.Id is required
            usuario.Tipo = GetString("Tipo");

            if (usuario.Tipo != null)
                usuario.Roles = new List<string> { usuario.Tipo };

            // IdSupervisor basado en Tipo
            if (string.Equals(usuario.Tipo, "cliente", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(usuario.Tipo, "asesor", StringComparison.OrdinalIgnoreCase))
            {
                usuario.IdSupervisor = GetInt("IdRelacionado");
            }

            usuario.Nombre = GetString("Nombre");
            usuario.PrimerApellido = GetString("PrimerApellido");
            usuario.SegundoApellido = GetString("SegundoApellido");
            usuario.FechaNacimiento = GetDateTime("FechaNacimiento");
            usuario.CorreoElectronico = GetString("CorreoElectronico");
            usuario.Direccion = GetString("Direccion");
            usuario.FotoPerfil = GetString("FotoPerfil");
            usuario.DocumentoContrato = GetString("RutaContrato");
            usuario.Contrasena = GetString("Contrasena");

            // Saldo basado en Tipo
            if (string.Equals(usuario.Tipo, "cliente", StringComparison.OrdinalIgnoreCase))
            {
                usuario.Saldo = GetDouble("Saldo");
            }

            usuario.Estado = GetBool("Estado") ?? false;
            usuario.FechaRegistro = GetDateTime("FechaRegistro");
            usuario.UltimoAcceso = GetDateTime("UltimoAcceso");

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
