using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.PayPalDTO
{
    public class SenderBatchHeader
    {
        public string SenderBatchId { get; set; }
        public string EmailSubject { get; set; }
        public string EmailMessage { get; set; }
    }
}
