using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class API_Response
    {
        public object? Data { get; set; } // resultado de datos en case de se un GET
        public required string Result { get; set; } // OK, ERROR
        public string? Message { get; set; } // Mensaje de error
    }
}
