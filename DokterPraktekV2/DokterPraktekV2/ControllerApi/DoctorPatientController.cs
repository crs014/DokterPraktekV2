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
        private DokterPraktekEntities1 db = new DokterPraktekEntities1();
        public IEnumerable<VM_patient> Get(int doctorId)
        {
            var data = db.histories.Where(a => a.doctorId == doctorId).DistinctBy(a => a.patientId).Select(e => new VM_patient
            {
                id = e.patientId,
                name = e.patient.name,
                address = e.patient.homeAddress,
                phone = e.patient.phone,
                gender = e.patient.gender,
                photo = e.patient.photo,
                dateTime = e.patient.registerDatetime
            }).ToList();
            return data;
        }
    }
}
