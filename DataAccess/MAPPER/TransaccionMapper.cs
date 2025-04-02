using DataAccess.DAO;
using DTO;
using DTO.TransaccionDTO;
using DTO.UsuarioDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.MAPPER
{
    public class TransaccionMapper : ICrudQueries, IObjectMapper
    {
        public SqlOperation GetCreateQuery(BaseClass entity)
        {
            throw new NotImplementedException();
        }

        public SqlOperation GetDeleteQuery(int Id)
        {
            throw new NotImplementedException();
        }

        public SqlOperation GetRetrieveAllQuery()
        {
            throw new NotImplementedException();
        }

        public SqlOperation GetRetrieveByEmailQuery(string email)
        {
            throw new NotImplementedException();
        }

        public SqlOperation GetRetrieveByIdQuery(int idDummy)
        {
            SqlOperation operation = new SqlOperation();
            operation.procedureName = "SP_SELECT_ALL_TAX_AND_COMMISIONS";

            return operation;
        }

        public SqlOperation GetUpdateQuery(BaseClass entity)
        {
            SqlOperation operation = new SqlOperation();
            operation.procedureName = "SP_UPDATE_TAX_AND_COMMISIONS";

            var cargosExtra = entity as CargosExtra;

            if (cargosExtra == null)
                throw new ArgumentException("Entity must be of type Usuario");

            // Required ID
            operation.AddDoubleParameter("ComisionTransaccion", cargosExtra.ComisionTransaccion);
            operation.AddDoubleParameter("ComisionAsesor", cargosExtra.ComisionAsesor);
            operation.AddDoubleParameter("ComisionAsesorGanancia", cargosExtra.ComisionAsesorGanancia);
            operation.AddDoubleParameter("ComisionAsesorPerdida", cargosExtra.ComisionAsesorPerdida);
            operation.AddDoubleParameter("ImpuestoSobreGanancia", cargosExtra.ImpuestoSobreGanancia);
            operation.AddDoubleParameter("TarifaMinimaTransaccion", cargosExtra.TarifaMinimaTransaccion);

            return operation;
        }

        public CargosExtra MapObject(Dictionary<string, object> objectRow)
        {
            T GetValue<T>(string key, T defaultValue = default)
            {
                if (!objectRow.ContainsKey(key) || objectRow[key] == null)
                    return defaultValue;

                try { return (T)Convert.ChangeType(objectRow[key], typeof(T)); }
                catch { return defaultValue; }
            }

            string GetString(string key)
            {
                if (objectRow[key] == null)
                    return null;

                return objectRow[key].ToString();
            }

            // Mapper

            CargosExtra cargosExtra = new CargosExtra()
            {
                ComisionTransaccion = GetValue<Double>("ComisionTransaccion"),
                ComisionAsesor = GetValue<Double>("ComisionAsesor"),
                ComisionAsesorGanancia = GetValue<Double>("ComisionAsesorGanancia"),
                ComisionAsesorPerdida = GetValue<Double>("ComisionAsesorPerdida"),
                ImpuestoSobreGanancia = GetValue<Double>("ImpuestoSobreGanancia"),
                TarifaMinimaTransaccion = GetValue<Double>("TarifaMinimaTransaccion")
            };

            return cargosExtra;
        }

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

        BaseClass IObjectMapper.MapObject(Dictionary<string, object> objectRow)
        {
            return MapObject(objectRow);
        }
    }

}
