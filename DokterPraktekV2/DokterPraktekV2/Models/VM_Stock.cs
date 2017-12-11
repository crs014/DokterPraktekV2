using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DokterPraktekV2.Models
{
    public class VM_Stock
    {
        public int id { get; set; }
        public int doctorId { get; set; }
        public string nameMedicine { get; set; }
        public decimal price { get; set; }
        public DateTime dateIn { get; set; }
        public DateTime expired { get; set; }
        public string nameDocter { get; set; }
        public Nullable<int> inStock { get; set; }
        public Nullable<int> outStock { get; set; }
        public Nullable<int> remainStock { get; set; }
    }
}