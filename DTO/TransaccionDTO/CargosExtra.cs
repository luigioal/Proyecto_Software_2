using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.TransaccionDTO
{
    public class CargosExtra: BaseClass
    {
        public double ComisionTransaccion { get; set; }
        public double ComisionAsesor { get; set; }
        public double ComisionAsesorPerdida { get; set; }
        public double ComisionAsesorGanancia { get; set; }
        public double ImpuestoSobreGanancia { get; set; }
        public double TarifaMinimaTransaccion { get; set; }
    }
}
