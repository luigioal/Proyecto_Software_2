using DataAccess.CRUD;
using DTO;
using DTO.TransaccionDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;


namespace AppLogic.TransaccionAdmin
{
    public class TransaccionAdmin
    {
        private TransaccionCrud _transaccionCrud;

        public TransaccionAdmin()
        {
            _transaccionCrud = new TransaccionCrud();
        }

        public CargosExtra ReturnCargosExtra()
        {
            return _transaccionCrud.RetrieveById<CargosExtra>(1);
        }
    }
}
