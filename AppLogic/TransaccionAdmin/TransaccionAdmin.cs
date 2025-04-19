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

        // Agregación de Validación de Saldo/Depositos /Paypal

        public bool ValidarSaldoSuficiente(int idUsuario, double monto)
        {
            var usuarioAdmin = new AppLogic.UsuarioAdmin.UsuarioAdmin();
            var usuario = usuarioAdmin.ReturnUsuarioById(idUsuario);

            return usuario.Saldo >= monto;
        }

        public bool ProcesarRetiro(int idUsuario, double monto)
        {
            // Validar saldo
            if (!ValidarSaldoSuficiente(idUsuario, monto))
            {
                return false;
            }

            // Actualizar saldo
            var usuarioAdmin = new AppLogic.UsuarioAdmin.UsuarioAdmin();
            var usuario = usuarioAdmin.ReturnUsuarioById(idUsuario);
            usuario.Saldo -= monto;

            usuarioAdmin.UpdateUsuario(usuario);

            // Registrar transacción
            // Implementación de registro en base de datos

            return true;
        }

        public bool ProcesarDeposito(int idUsuario, double monto)
        {
            if (monto <= 0 || monto > 10000) // para veficar limites maximos: Límite de $10,000
            {
                return false;
            }
            // Actualizar saldo
            var usuarioAdmin = new AppLogic.UsuarioAdmin.UsuarioAdmin();
            var usuario = usuarioAdmin.ReturnUsuarioById(idUsuario);
            usuario.Saldo += monto;

            usuarioAdmin.UpdateUsuario(usuario);

            // Registrar transacción
            // Implementación de registro en base de datos

            return true;
        }
    }
}
