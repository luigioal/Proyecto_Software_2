using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.TransaccionDTO
{
    public class ResultadoTransaccion
    {
        public bool Exito { get; set; }
        public string Mensaje { get; set; }
        public double? NuevoSaldo { get; set; }
    }
}