using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DokterPraktekV2;
using DokterPraktekV2.Models;
using PagedList;

namespace DokterPraktekV2.ControllerApi
{
    public class PatientController : ApiController
    {
        private DokterPraktekEntities db = new DokterPraktekEntities();
        public IEnumerable<VM_patient> Get()
        {
            var data = db.Patients.Select(e => new VM_patient
            {
                id = e.ID,
                name = e.Name,
                address = e.Address,
                phone = e.PhoneNumber,
                gender = e.Gender,
                photo = e.Photo,
                dateTime = e.CreatedDate
            }).ToList();
            return data;
        }

        
        // GET api/values/5
        public IHttpActionResult Get(int id)
        {
            var data = db.Patients.FirstOrDefault(b => b.ID == id);
            VM_patient patient = new VM_patient();
            patient.id = data.ID;
            patient.name = data.Name; 
            patient.address = data.Address;
            patient.phone = data.PhoneNumber;
            patient.gender = data.Gender;
            patient.photo = data.Photo;
            patient.dateTime = data.CreatedDate;
            return Ok(patient);
        }

        


        // POST api/values
        /*public void Post([FromBody]string value)
        {
        }*/

        // PUT api/values/5
        /*public void Put(int id, [FromBody]string value)
        {
        }*/

        // DELETE api/values/5
        /*public void Delete(int id)
        {
        }*/
    }
}
