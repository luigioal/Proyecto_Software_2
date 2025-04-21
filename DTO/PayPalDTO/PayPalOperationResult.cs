using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.PayPalDTO
{
    public class PayPalOperationResult
    {
        public bool Success { get; set; }
        public string ApprovalUrl { get; set; }
        public string ErrorMessage { get; set; }
    }
}
