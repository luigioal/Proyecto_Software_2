using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.UsuarioDTO
{
    public class Usuario : BaseClass
    {
        public string Tipo{ get; set; }
        public int? IdAdmin { get; set; }
        public int? IdAsesor { get; set; }
        public string Nombre { get; set; }
        public string PrimerApellido { get; set; }
        public string? SegundoApellido { get; set; }
        public DateTime FechaNacimiento{ get; set; }
        public string CorreoElectronico { get; set; }
        public string? Direccion { get; set; }
        public string? FotoPerfil { get; set; }
        public string? DocumentoContrato { get; set; }
        public string Contrasena { get; set; }
        public double? Saldo { get; set; }
        public bool Estado { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime? UltimoAcceso { get; set; }
    }
}
