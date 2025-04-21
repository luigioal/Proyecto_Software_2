using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.PayPalDTO
{
    public class PayPalTransactionDTO
    {
        public int UsuarioId { get; set; }
        public double Monto { get; set; }
        public string PayPalTransactionId { get; set; }
        public string Estado { get; set; } // "Pendiente", "Completada", "Fallida"
        public DateTime FechaCreacion { get; set; }
        public string Descripcion { get; set; }
}
}
