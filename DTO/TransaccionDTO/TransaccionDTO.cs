using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.TransaccionDTO
{
    public class TransaccionDTO : BaseClass
    {
        public int UsuarioId { get; set; }
        public double Monto { get; set; }
        public double Comision { get; set; }
        public string Tipo { get; set; } // "DepositoPayPal", "Retiro"
        public string Referencia { get; set; } // ID de transacción PayPal
        public string Estado { get; set; } // "Pendiente", "Completado", "Fallido"
        public DateTime Fecha { get; set; } = DateTime.Now;
    }
}
