using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.SeguridadDTO
{
    public class PresignRequest
    {
        public string FileName { get; set; }        // e.g., "document.pdf"
        public string ContentType { get; set; }     // e.g., "application/pdf"
        public long? FileSize { get; set; }         // Optional: for size validation
        public string UserId { get; set; }          // Optional: for user-specific uploads

    }
}
