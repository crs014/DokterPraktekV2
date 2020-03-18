using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DokterPraktekV2.Models
{
    public class VM_transaction
    {
        public int historyId { get; set; }
        public decimal amount { get; set; }
        public string DoctorName { get; set; }
        public Nullable<decimal> alreadyPay { get; set; }
        public DateTime dateHistory { get; set; }
        public int patientId { get; set; }
        public string patientName { get; set; }
    }

    public class VM_totalTrans
    {
        public Nullable<decimal> alreadyPay { get; set; }
    }
}