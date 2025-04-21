using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.PayPalDTO
{
    public class SolicitudDepositoDTO
    {
        public int UsuarioId { get; set; }
        public double Monto { get; set; }
        public string Descripcion { get; set; }

    }
}
