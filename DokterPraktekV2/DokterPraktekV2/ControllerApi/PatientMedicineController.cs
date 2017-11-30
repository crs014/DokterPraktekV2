using DokterPraktekV2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DokterPraktekV2.ControllerApi
{
    public class PatientMedicineController : ApiController
    {
        private DokterPraktekEntities1 db = new DokterPraktekEntities1();
        public IEnumerable<VM_patientMedicine> Get(int id)
        {
            var data = db.patientMedicines.Where(a => a.historyId == id).Select(e => new VM_patientMedicine
            {
                id = e.id,
                historyId = e.historyId,
                medicineName = e.medicine.name,
                price = e.medicinePrice,
                description = e.describe,
                quantity = e.quantity
            }).ToList();
            return data;
        }
    }
}
