using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.SeguridadDTO
{
    public class PresignedUrlResponse
    {
        public required string presignedUrl { get; set; }
        public required string publicUrl { get; set; }
    }
}
