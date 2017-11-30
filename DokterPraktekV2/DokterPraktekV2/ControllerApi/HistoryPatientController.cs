using DokterPraktekV2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DokterPraktekV2.ControllerApi
{

    public class HistoryPatientController : ApiController
    {

        private DokterPraktekEntities1 db = new DokterPraktekEntities1();

        public IEnumerable<VM_history> Get(int id)
        {

            var data = db.histories.Where(a => a.patientId == id).Select(e => new VM_history
            {
                id = e.id,
                doctorId = e.doctorId,
                doctorName = e.doctor.name,
                sickness = e.sickness,
                description = e.descriptionInfo,
                date = e.checkupDate,
                checkupPrice = e.checkupPrice,
                patientId = e.patientId,
                patientName = e.patient.name,
                gender = e.patient.gender
            }).ToList();

            return data;
        }
    }
}
