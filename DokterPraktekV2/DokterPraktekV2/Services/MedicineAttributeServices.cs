using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DokterPraktekV2.Services
{
    public class MedicineAttributeServices
    {
        private DokterPraktekEntities db = new DokterPraktekEntities();
        public List<MedicineAttribute> GetListOfAttribute(string AttributeName)
        {
            var getData = db.MedicineAttributes.Distinct();
            return getData.Where(e => e.AttributeName == AttributeName).ToList();
        }
        
    }
}