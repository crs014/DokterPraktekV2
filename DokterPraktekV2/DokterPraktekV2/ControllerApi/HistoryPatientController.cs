using DokterPraktekV2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;

namespace DokterPraktekV2.ControllerApi
{

    public class HistoryPatientController : ApiController
    {

        private DokterPraktekEntities db = new DokterPraktekEntities();

        public IEnumerable<VM_history> Get(int ? id,string doctorID)
        {
            var data = db.MedicalHistories.Where(a => a.PatientID == id && a.DoctorID == doctorID.ToString()).Select(e => new VM_history
            {
                id = e.ID,
                doctorId = e.DoctorID,
                doctorName = db.doctors.Where(o=>o.userId == e.DoctorID).FirstOrDefault().name,
                sickness = e.Sickness,
                description = e.DescriptionInfo,
                date = e.CheckUpDate,
                checkupPrice = e.CheckUpPrice,
                patientId = e.PatientID,
                patientName = e.Patient.Name,
                gender = e.Patient.Gender
            }).ToList();

            return data;
        }
    }
}
