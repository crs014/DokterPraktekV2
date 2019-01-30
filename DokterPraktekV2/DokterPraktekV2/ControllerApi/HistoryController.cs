using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DokterPraktekV2.Models;
using Microsoft.AspNet.Identity;
namespace DokterPraktekV2.ControllerApi
{
    public class HistoryController : ApiController
    {
        private DokterPraktekEntities db = new DokterPraktekEntities();
      

        public IHttpActionResult Get(int id, string doctorID)
        {            
            var data = db.MedicalHistories.FirstOrDefault(b => b.ID == id && b.DoctorID == doctorID.ToString());
            VM_history history = new VM_history();
            history.doctorId = data.DoctorID;
            history.doctorName = db.doctors.Where(o => o.userId == data.DoctorID).FirstOrDefault().name;
            history.sickness = data.Sickness;
            history.checkupPrice = data.CheckUpPrice;
            history.description = data.DescriptionInfo;
            history.date = data.CheckUpDate;
            history.patientId = data.PatientID;
            history.patientName = data.Patient.Name; 
            return Ok(history);
        }
    }
}
