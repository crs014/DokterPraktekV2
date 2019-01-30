using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DokterPraktekV2;
using DokterPraktekV2.Models;
using Microsoft.Ajax.Utilities;

namespace DokterPraktekV2.ControllerApi
{
    public class DoctorPatientController : ApiController
    {
        private DokterPraktekEntities db = new DokterPraktekEntities();
        public IEnumerable<VM_patient> Get(string doctorId)
        {
            var data = db.MedicalHistories.Where(a => a.DoctorID == doctorId.ToString()).DistinctBy(a => a.PatientID).Select(e => new VM_patient
            {
                id = e.PatientID,
                name = e.Patient.Name,
                address = e.Patient.Address,
                phone = e.Patient.PhoneNumber,
                gender = e.Patient.Gender,
                photo = e.Patient.Photo,
                dateTime = e.Patient.CreatedDate
            }).ToList();
            return data;
        }
    }
}
