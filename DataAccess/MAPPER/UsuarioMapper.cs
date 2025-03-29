using DataAccess.DAO;
using DTO;
using DTO.UsuarioDTO;
using System.Net.Http.Headers;


namespace DataAccess.MAPPERS
{
    public class UsuarioMapper : ICrudQueries, IObjectMapper
    {
        // CREATE operation
        public SqlOperation GetCreateQuery(BaseClass entity)
        {
            var usuario = entity as Usuario;
            if (usuario == null)
                throw new ArgumentException("Entity must be of type Usuario");

            SqlOperation operation = new SqlOperation();
            operation.procedureName = "SP_CREATE_USER";

            // Required parameters
            operation.AddVarcharParameter("Tipo", usuario.Tipo ?? "Cliente"); //Default es cliente si no se proporciona
            operation.AddVarcharParameter("Nombre", usuario.Nombre ?? string.Empty);
            operation.AddVarcharParameter("PrimerApellido", usuario.PrimerApellido ?? string.Empty);
            operation.AddDateTimeParameter("FechaNacimiento", usuario.FechaNacimiento ?? DateTime.Now);
            operation.AddVarcharParameter("CorreoElectronico", usuario.CorreoElectronico ?? string.Empty);
            operation.AddVarcharParameter("Contrasena", usuario.Contrasena ?? string.Empty);

            // Optional parameters
            if (usuario.SegundoApellido != null)
                operation.AddVarcharParameter("SegundoApellido", usuario.SegundoApellido);

            if (usuario.Direccion != null)
                operation.AddVarcharParameter("Direccion", usuario.Direccion);

            if (usuario.FotoPerfil != null)
                operation.AddVarcharParameter("FotoPerfil", usuario.FotoPerfil);

            operation.AddBooleanParameter("Estado", usuario.Estado ?? false);

            if (usuario.IdSupervisor.HasValue)
                operation.AddIntegerParameter("IdSupervisor", usuario.IdSupervisor.Value);

            if (usuario.Saldo.HasValue && usuario.Tipo?.ToLower() == "cliente")
                operation.AddDoubleParameter("SaldoInicial", usuario.Saldo.Value);

            return operation;
        }

        // UPDATE operation
        public SqlOperation GetUpdateQuery(BaseClass entity)
        {
            var usuario = entity as Usuario;
            if (usuario == null)
                throw new ArgumentException("Entity must be of type Usuario");

            SqlOperation operation = new SqlOperation();
            operation.procedureName = "SP_UPDATE_USER";

            // Required ID
            operation.AddIntegerParameter("UsuarioID", usuario.Id);

            // Optional update parameters 
            if (!string.IsNullOrEmpty(usuario.Nombre))
                operation.AddVarcharParameter("Nombre", usuario.Nombre);

            if (!string.IsNullOrEmpty(usuario.PrimerApellido))
                operation.AddVarcharParameter("PrimerApellido", usuario.PrimerApellido);

            //Pueden ser null entonces siempre los inlcuimos
            operation.AddVarcharParameter("SegundoApellido", usuario.SegundoApellido);
            operation.AddVarcharParameter("Direccion", usuario.Direccion);
            operation.AddVarcharParameter("FotoPerfil", usuario.FotoPerfil);

            if (!string.IsNullOrEmpty(usuario.Contrasena))
                operation.AddVarcharParameter("Contrasena", usuario.Contrasena);

            operation.AddBooleanParameter("Estado", usuario.Estado ?? false);

            // Actualizar relacion supervisor si es proporcionado
            if (usuario.IdSupervisor.HasValue)
            {
                operation.AddIntegerParameter("IdSupervisor", usuario.IdSupervisor.Value);
                operation.AddBooleanParameter("CambiarSupervisor", true);
            }

            return operation;
        }

        // DELETE operation soft delete
        public SqlOperation GetDeleteQuery(int Id)
        {
            SqlOperation operation = new SqlOperation();
            operation.procedureName = "SP_DEACTIVATE_USER";
            operation.AddIntegerParameter("UsuarioID", Id);

            return operation;
        }

        // RETRIEVE ALL 
        public SqlOperation GetRetrieveAllQuery()
        {
            SqlOperation operation = new SqlOperation();
            operation.procedureName = "SP_SELECT_ALL_USERS";

            return operation;
        }

        // RETRIEVE ALL BY SUPERVISOR
        public SqlOperation GetRetrieveAllQuery(int idSuper)
        {
            SqlOperation operation = new SqlOperation();
            operation.procedureName = "SP_SELECT_ALL_USERS_BY_SUPER";
            operation.AddIntegerParameter("IdSuper", idSuper);

            return operation;
        }

        // RETRIEVE BY ID
        public SqlOperation GetRetrieveByIdQuery(int Id)
        {
            SqlOperation operation = new SqlOperation();
            operation.procedureName = "SP_SELECT_USER_BY_ID";
            operation.AddIntegerParameter("UsuarioID", Id);

            return operation;
        }

        // RETRIEVE BY EMAIL 
        public SqlOperation GetRetrieveByEmailQuery(string email)
        {
            SqlOperation operation = new SqlOperation();
            operation.procedureName = "SP_SELECT_USER_BY_EMAIL";
            operation.AddVarcharParameter("Email", email);

            return operation;
        }

        // Operation para servir la peticion del crud de modificar un rol a un usuario indicado/ Si ya lo tiene se remueve o viceversa 
        public SqlOperation GetUpdateRolQuery(int idUsuario, string rol)
        {
            SqlOperation operation = new SqlOperation();
            operation.procedureName = "SP_UPDATE_USER_ROLES";
            operation.AddIntegerParameter("UsuarioID", idUsuario);
            operation.AddVarcharParameter("NombrePermiso", rol);

            return operation;
        }

        // Operation para servir la peticion del crud de activar/desactivar a un usuario indicado
        public SqlOperation GetActivateDeactivateQuery(int idUsuario, bool nuevoEstado)
        {
            SqlOperation operation = new SqlOperation();

            if (nuevoEstado)
            {
                operation.procedureName = "SP_ACTIVATE_USER";
                operation.AddIntegerParameter("UsuarioID", idUsuario);
                return operation;
            }
            else
            {
                operation.procedureName = "SP_DEACTIVATE_USER";
                operation.AddIntegerParameter("UsuarioID", idUsuario);
                return operation;
            }
        }

        // MAP OBJECT method 
        public BaseClass MapObject(Dictionary<string, object> objectRow)
        {
            Usuario usuario = new Usuario();

           
            T GetValue<T>(string key, T defaultValue = default)
            {
                if (!objectRow.ContainsKey(key) || objectRow[key] == null || objectRow[key] == DBNull.Value)
                    return defaultValue;

                try { return (T)Convert.ChangeType(objectRow[key], typeof(T)); }
                catch { return defaultValue; }
            }

            string GetString(string key)
            {
                if (!objectRow.ContainsKey(key) || objectRow[key] == null || objectRow[key] == DBNull.Value)
                    return null;

                return objectRow[key].ToString();
            }

            DateTime? GetDateTime(string key)
            {
                if (!objectRow.ContainsKey(key) || objectRow[key] == null || objectRow[key] == DBNull.Value)
                    return null;

                try { return DateTime.Parse(objectRow[key].ToString()); }
                catch { return null; }
            }

            // Mapper
            usuario.Id = GetValue<int>("UsuarioID");
            usuario.Tipo = GetString("Tipo");

            // Desagregacion de roles a partir del string del stored procedure
            usuario.Roles = GetString("Permisos").Split(',')
                    .Select(s => s.Trim())
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .ToList();


            // Id Supervisor
            usuario.IdSupervisor = GetValue<int?>("IdSupervisor");

            usuario.Nombre = GetString("Nombre");
            usuario.PrimerApellido = GetString("PrimerApellido");
            usuario.SegundoApellido = GetString("SegundoApellido");
            usuario.FechaNacimiento = GetDateTime("FechaNacimiento");
            usuario.CorreoElectronico = GetString("CorreoElectronico");
            usuario.Direccion = GetString("Direccion");
            usuario.FotoPerfil = GetString("FotoPerfil");
            usuario.DocumentoContrato = GetString("DocumentoContrato") ?? GetString("RutaContrato");
            usuario.Contrasena = GetString("Contrasena");

            // Saldo
            if (objectRow.ContainsKey("Saldo") && objectRow["Saldo"] != null && objectRow["Saldo"] != DBNull.Value)
            {
                try { usuario.Saldo = Convert.ToDouble(objectRow["Saldo"]); }
                catch { usuario.Saldo = null; }
            }

            usuario.Estado = GetValue<bool>("Estado");
            usuario.FechaRegistro = GetDateTime("FechaRegistro") ?? DateTime.Now;
            usuario.UltimoAcceso = GetDateTime("UltimoAcceso");

            return usuario;
        }

        // MAP OBJECT LIST 
        public List<BaseClass> MapObjectList(List<Dictionary<string, object>> objectList)
        {
            var list = new List<BaseClass>();

            foreach (var objectRow in objectList)
            {
                var usuario = MapObject(objectRow);
                list.Add(usuario);
            }

            return list;
        }
    }
}
