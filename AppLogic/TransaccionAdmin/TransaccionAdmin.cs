using AppLogic.FinanzaAdmin;
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
        private FinanceConnector _connector;

        public TransaccionAdmin()
        {
            _transaccionCrud = new TransaccionCrud();
            _connector = new FinanceConnector();
        }

        public CargosExtra ReturnCargosExtra()
        {
            return _transaccionCrud.RetrieveCargosExtra();
        }

        public void ModifyCargoExtra(CargosExtra cargosExtra)
        {
            _transaccionCrud.Update(cargosExtra);
        }

        public List<InversionCard> ReturnInversiones(int idUsuario,
                                                     string tipo, // Activo, Venta   
                                                     DateTime fechaInicio,
                                                     DateTime fechaFin)
        {
                List<InversionCard> inversiones = _transaccionCrud.RetrieveAll<InversionCard>(idUsuario, tipo, fechaInicio, fechaFin);

                inversiones.ForEach(inversion =>
                {
                    inversion.Tipo = tipo;
                    inversion.FotoLogo = $"https://images.financialmodelingprep.com/symbol/{inversion.Simbolo}.png";
                    //inversion.PrecioUnitarioActual = _connector.GetCurrentPrice(inversion.Simbolo);
                });

                return inversiones; 

        }
    }
}
