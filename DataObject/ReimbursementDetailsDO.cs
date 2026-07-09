using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObject
{
    public class ReimbursementDetailsDO
    {
        public string reimbursementNumber { get; set; }
        public string claimType { get; set; }
        public DateTime claimDate { get; set; }
        public string paymentMonth { get; set; }
        public decimal claimAmount { get; set; }
        public string document { get; set; }
        public string remarks { get; set; }
        public string status { get; set; }
    }
}
