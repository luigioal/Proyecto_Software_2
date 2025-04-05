using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.TransaccionDTO
{
    public class Inversion
    {
        public string Tipo { get; set; }
        public int EjecutorID { get; set; }
        public string Simbolo { get; set; } 
        public string Nombre { get; set; }  
        public double Cantidad { get; set; }
        public double PrecioUnitario { get; set; }
        public DateTime FechaOperacion { get; set; }
        public string Estado { get; set; }


    }
}
