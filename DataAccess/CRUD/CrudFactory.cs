using DataAccess.DAO;
using DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.CRUD
{
    public abstract class CrudFactory
    {
        protected SqlDao dao;

        //public abstract void Create(BaseClass entity);
        //public abstract void Update(BaseClass entity);
        public abstract void Delete(int Id);
        public abstract List<T> RetrieveAll<T>();
        //public abstract T RetrieveById<T>(int Id);
        public abstract T RetrieveByEmail<T>(string email);
    }
}
