using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DokterPraktekV2.Models
{
    public class VM_transactionInput
    {
        [Required]
        public decimal amount { get; set; }
    }
}