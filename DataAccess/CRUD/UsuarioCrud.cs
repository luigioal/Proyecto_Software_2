using DataAccess.DAO;
using DataAccess.MAPPERS;
using DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.CRUD
{
    public class UsuarioCrud : CrudFactory
    {
        private UsuarioMapper mapper;

        public UsuarioCrud() : base() { 
            
            mapper = new UsuarioMapper();
            dao = SqlDao.GetInstance();
        
        }

        public override void Delete(int Id)
        {
            SqlOperation operation = mapper.GetDeleteQuery(Id);
            dao.ExecuteStoredProcedure(operation);
        }

        public override List<T> RetrieveAll<T>()
        {
            List<T> list = new List<T>();
            SqlOperation operation = mapper.GetRetrieveAllQuery();

            List<Dictionary<string, object>> dataResults = dao.ExecuteStoredProcedureWithQuery(operation);

            if (dataResults.Count > 0)
            {
                var dtObjects = mapper.MapObjectList(dataResults);
                foreach (var obj in dtObjects)
                {
                    list.Add((T)Convert.ChangeType(obj, typeof(T)));
                }
            }

            return list;
        }

        public List<T> RetrieveAll<T>(int idSuper)
        {
            List<T> list = new List<T>();
            SqlOperation operation = mapper.GetRetrieveAllQuery(idSuper);

            List<Dictionary<string, object>> dataResults = dao.ExecuteStoredProcedureWithQuery(operation);

            if (dataResults.Count > 0)
            {
                var dtObjects = mapper.MapObjectList(dataResults);
                foreach (var obj in dtObjects)
                {
                    list.Add((T)Convert.ChangeType(obj, typeof(T)));
                }
            }

            return list;
        }

        public override T RetrieveByEmail<T>(string email)
        {
            List<T> listResults = new List<T>();
            SqlOperation operation = mapper.GetRetrieveByEmailQuery(email);

            List<Dictionary<string, object>> vac = dao.ExecuteStoredProcedureWithQuery(operation);

            var dtoObjects = mapper.MapObjectList(vac);
            var obj = dtoObjects.FirstOrDefault();

            return (T)Convert.ChangeType(obj, typeof(T));
        }

        public override void Update(BaseClass entity)
        {
            // Get the Operation object from the mapper instance
            SqlOperation operation = _mapper.GetUpdateQuery(entity);
            // Ask the DAO to perform the operation in the 
            dao.ExecuteStoredProcedureWithQuery(operation);
        }
    }
}
