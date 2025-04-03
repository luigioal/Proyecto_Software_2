using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.TransaccionDTO
{
    public class InversionCard: BaseClass
    {
        public string Tipo { get; set; } // Activo, venta * From Input
        public string Instrumento { get; set; }// ETF, Accion * From DB
        public string Nombre { get; set; }  // * From DB
        public string Simbolo { get; set; } // * From DB
        public DateTime? FechaVenta { get; set; } // * From DB
        public DateTime? FechaUltimaCompra { get; set; } // * From DB
        public double Cantidad { get; set; } // * From DB
        public double? GananciaEjecutada { get; set; } // * From DB for Venta
        public double? PrecioPromedioCompra { get; set; } // Precio promedio de compra * From DB
        public string FotoLogo { get; set; } // * From TransaccionAdmin
        public double? PrecioUnitarioVenta { get; set; } // * From DB
        public double? PrecioUnitarioActual { get; set; } // * From FinanzaConnector
    }
}
