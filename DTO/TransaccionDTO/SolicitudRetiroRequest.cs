using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.TransaccionDTO
{
    public class SolicitudRetiroRequest
    {
        public int UsuarioId { get; set; }
        public double Monto { get; set; }
    }
}
