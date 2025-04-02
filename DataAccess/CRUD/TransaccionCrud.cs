using DataAccess.DAO;
using DataAccess.MAPPER;
using DTO;
using DTO.TransaccionDTO;
using DTO.UsuarioDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.CRUD
{
    public class TransaccionCrud : CrudFactory
    {
        private TransaccionMapper mapper;

        public TransaccionCrud() : base()
        {
            mapper = new TransaccionMapper();
            dao = SqlDao.GetInstance();
        }

        public override void Create(BaseClass entity)
        {
            throw new NotImplementedException();
        }

        public override void Delete(int Id)
        {
            throw new NotImplementedException();
        }

        public override List<T> RetrieveAll<T>()
        {
            throw new NotImplementedException();
        }

        public override T RetrieveByEmail<T>(string email)
        {
            throw new NotImplementedException();
        }

        public override T RetrieveById<T>(int Id)
        {
            SqlOperation operation = mapper.GetRetrieveByIdQuery(Id);
            CargosExtra resultDTO;

            List<Dictionary<string, object>> dataResults = dao.ExecuteStoredProcedureWithQuery(operation);
            if (dataResults.Count > 0)
            {
                resultDTO = (CargosExtra)mapper.MapObject(dataResults[0]);
            }
            else
            {
                resultDTO = new CargosExtra()
                {
                    ComisionAsesor = 0.0,
                    ComisionAsesorGanancia = 0.0,
                    ComisionAsesorPerdida = 0.0,
                    ComisionTransaccion = 0.0,
                    ImpuestoSobreGanancia = 0.0,
                    TarifaMinimaTransaccion = 0.0
                };
            }
            return (T)Convert.ChangeType(resultDTO, typeof(T));
        }

        public override void Update(BaseClass entity)
        {
            // Get the Operation object from the mapper instance
            SqlOperation operation = mapper.GetUpdateQuery(entity);
            // Ask the DAO to perform the operation in the 
            dao.ExecuteStoredProcedureWithQuery(operation);
        }
    }
}

