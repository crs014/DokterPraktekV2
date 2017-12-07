using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DokterPraktekV2.Models
{
    public class VM_Stock
    {
        public int id { get; set; }
        public string name { get; set; }
        public Nullable<int> inStock { get; set; }
        public Nullable<int> outStock { get; set; }
        public Nullable<int> remainStock { get; set; }
    }
}