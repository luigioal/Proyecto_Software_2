using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.UsuarioDTO
{
    public class Asesor: Usuario
    {
        public int IdAdmin { set; get; }
        public String UrlContrato { set; get; }

        public int TotalClientes { set; get; }

        public Double TotalComisiones { set; get; }

    }
}
