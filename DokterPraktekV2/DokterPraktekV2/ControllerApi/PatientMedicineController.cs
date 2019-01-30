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
        private DokterPraktekEntities db = new DokterPraktekEntities();
        public IEnumerable<VM_patientMedicine> Get(int id)
        {
            var data = db.PatientMedicines.Where(a => a.MedicalHistoryID == id).Select(e => new VM_patientMedicine
            {
                id = e.ID,
                historyId = e.MedicalHistoryID,
                medicineName = e.Medicine.Name,
                price = e.MedicalPrice,
                description = e.Description,
                quantity = e.Quantity
            }).ToList();
            return data;
        }
    }
}
