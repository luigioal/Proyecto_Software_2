using AppLogic.ConnectorsAdmin;
using AppLogic.SeguridadAdmin;
using AppLogic.UsuarioAdmin;
using DataAccess.CRUD;
using DTO;
using DTO.TransaccionDTO;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using Amazon.S3;



namespace AppLogic.TransaccionAdmin
{
    public class TransaccionAdmin
    {
        private TransaccionCrud _transaccionCrud;
        private FinanceConnector _financeConnector;
        private readonly PayPalConnector _paypalConnector;
        private readonly Notificador _notificador;
        private readonly SeguridadAdministrador _seguridadAdmin;
        private readonly UsuarioAdministrador _usuarioAdmin;
        private readonly AwsConnector _awsConnector;
        private readonly IConfiguration _config;

        public TransaccionAdmin(
            TransaccionCrud transaccionCrud = null,
            FinanceConnector financeConnector = null,
            PayPalConnector paypalConnector = null,
            Notificador notificador = null,
            SeguridadAdministrador seguridadAdmin = null,
            IConfiguration config = null)
        {
            _config = config;
            _transaccionCrud = transaccionCrud ?? new TransaccionCrud();
            _financeConnector = financeConnector ?? new FinanceConnector();
            _paypalConnector = paypalConnector ?? new PayPalConnector(config);
            _notificador = notificador ?? new Notificador();
            _awsConnector = new AwsConnector(new AmazonS3Client(),config);
            _seguridadAdmin = seguridadAdmin ?? new SeguridadAdministrador(_notificador,_awsConnector);
            _usuarioAdmin = new UsuarioAdministrador();
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
        #region Implementación  - Depósito PayPal
        public async Task<ResultadoTransaccion> ProcesarDepositoPayPal(int usuarioId, double monto, string transactionId)
        {
            try
            {
                //  Validar monto positivo
                if (monto <= 0)
                    return new ResultadoTransaccion { Exito = false, Mensaje = "El monto debe ser mayor a cero" };

                // 1. Validar transacción PayPal
                var paypalValido = await _paypalConnector.VerifyPaymentAsync(transactionId);
                if (!paypalValido)
                    return new ResultadoTransaccion { Exito = false, Mensaje = "Transacción PayPal no válida" };

                // 2. Obtener usuario
                var usuario = _usuarioAdmin.ReturnUsuarioById(usuarioId);
                if (usuario == null)
                    return new ResultadoTransaccion { Exito = false, Mensaje = "Usuario no encontrado" };

                // 3. Calcular comisiones (usando cargos extra)
                var cargos = ReturnCargosExtra();
                double comisionTotal = CalcularComisionesDeposito(monto, cargos);

                // 4. Actualizar saldo
                usuario.Saldo += (monto - comisionTotal);
                _usuarioAdmin.UpdateUsuario(usuario);

                // 5. Registrar transacción
                RegistrarTransaccion(new TransaccionDTO
                {
                    UsuarioId = usuarioId,
                    Monto = monto,
                    Comision = comisionTotal,
                    Tipo = "DepositoPayPal",
                    Referencia = transactionId,
                    Estado = "Completado"
                });

                // 6. Enviar notificación (RF27)
                await _notificador.EnviarNotificacionTransaccion(
                    usuario.CorreoElectronico,
                    "Depósito PayPal",
                    monto,
                    $"Comisión: {comisionTotal.ToString("C")}");

                return new ResultadoTransaccion { Exito = true };
            }
            catch (Exception ex)
            {
                return new ResultadoTransaccion { Exito = false, Mensaje = $"Error: {ex.Message}" };
            }
        }

        private double CalcularComisionesDeposito(double monto, CargosExtra cargos)
        {
            // Lógica para calcular comisiones según tus reglas de negocio
            double comisionFija = cargos.TarifaMinimaTransaccion;
            double comisionPorcentual = monto * (cargos.ComisionTransaccion / 100);

            return Math.Max(comisionFija, comisionPorcentual); // Tomar el mayor entre fija y porcentual
        }
        #endregion

        #region Implementación - Retiro con validación de saldo y OTP
        public async Task<ResultadoTransaccion> ProcesarRetiroConOTP(int usuarioId, double monto, string otp)
        {
            try
            {
                // 1. Validar OTP 
                var usuario = _usuarioAdmin.ReturnUsuarioById(usuarioId);
                if (!_seguridadAdmin.Verify(usuario.CorreoElectronico, otp))
                    return new ResultadoTransaccion { Exito = false, Mensaje = "OTP inválido o expirado" };

                // 2. Validar saldo 
                if (!ValidarSaldoSuficiente(usuarioId, monto))
                    return new ResultadoTransaccion { Exito = false, Mensaje = "Saldo insuficiente" };

                // 3. Calcular comisiones
                var cargos = ReturnCargosExtra();
                double comisionTotal = CalcularComisionesRetiro(monto, cargos);
                double totalRetiro = monto + comisionTotal;

                // 4. Validar saldo nuevamente incluyendo comisiones
                if (!ValidarSaldoSuficiente(usuarioId, totalRetiro))
                    return new ResultadoTransaccion { Exito = false, Mensaje = "Saldo insuficiente para cubrir comisiones" };

                // 5. Actualizar saldo
                usuario.Saldo -= totalRetiro;
                _usuarioAdmin.UpdateUsuario(usuario);

                // 6. Registrar transacción
                RegistrarTransaccion(new TransaccionDTO
                {
                    UsuarioId = usuarioId,
                    Monto = monto,
                    Comision = comisionTotal,
                    Tipo = "Retiro",
                    Estado = "Completado"
                });

                // 7. Enviar notificación (RF27)
                await _notificador.EnviarNotificacionTransaccion(
                    usuario.CorreoElectronico,
                    "Retiro de fondos",
                    monto,
                    $"Comisión: {comisionTotal.ToString("C")}");

                return new ResultadoTransaccion { Exito = true };
            }
            catch (Exception ex)
            {
                return new ResultadoTransaccion { Exito = false, Mensaje = $"Error: {ex.Message}" };
            }
        }

       
       public void DoVenderInversion(Inversion inversion)
            {
                // Falta implementar la logica de este metodo
            }
        

        private void RegistrarTransaccion(TransaccionDTO transaccion)
        {
            if (transaccion == null)
                throw new ArgumentNullException(nameof(transaccion));

            _transaccionCrud.Create(transaccion);
        }


        private double CalcularComisionesRetiro(double monto, CargosExtra cargos)
        {
            // Lógica específica para comisiones de retiro
            return monto * (cargos.ComisionAsesor / 100) + cargos.TarifaMinimaTransaccion;
        }

        public bool ValidarSaldoSuficiente(int usuarioId, double monto)
        {
            var usuario = _usuarioAdmin.ReturnUsuarioById(usuarioId);
            return usuario?.Saldo >= monto;
        }
        #endregion

    }
}