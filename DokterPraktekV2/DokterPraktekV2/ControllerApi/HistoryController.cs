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
        private DokterPraktekEntities1 db = new DokterPraktekEntities1();
      

        public IHttpActionResult Get(int id, int doctorID)
        {            
            var data = db.histories.FirstOrDefault(b => b.id == id && b.doctorId == doctorID);
            VM_history history = new VM_history();
            history.doctorId = data.doctorId;
            history.doctorName = data.doctor.name;
            history.sickness = data.sickness;
            history.checkupPrice = data.checkupPrice;
            history.description = data.descriptionInfo;
            history.date = data.checkupDate;
            history.patientId = data.patientId;
            history.patientName = data.patient.name; 
            return Ok(history);
        }
    }
}
