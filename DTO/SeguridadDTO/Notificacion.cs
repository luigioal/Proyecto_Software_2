using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.SeguridadDTO
{
    public class Notificacion : BaseClass
    {
        public string Destinatario { get; set; }
        public string Asunto { get; set; }
        public string Cuerpo { get; set; }
        public bool EsHtml { get; set; } = true;
        public string Tipo { get; set; } // "Email", "SMS", etc.
        public DateTime FechaEnvio { get; set; }
        public bool EnvioExitoso { get; set; }
        public string MensajeError { get; set; }
    }
}
