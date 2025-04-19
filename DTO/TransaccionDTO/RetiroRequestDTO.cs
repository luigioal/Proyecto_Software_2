using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.TransaccionDTO
{
    public class RetiroRequestDTO
    {
        public int IdUsuario { get; set; }
        public double Monto { get; set; }
    }
}
