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
        private DokterPraktekEntities1 db = new DokterPraktekEntities1();
        public IEnumerable<VM_patient> Get()
        {
            var data = db.patients.Select(e => new VM_patient
            {
                id = e.id,
                name = e.name,
                address = e.homeAddress,
                phone = e.phone,
                gender = e.gender,
                photo = e.photo,
                dateTime = e.registerDatetime
            }).ToList();
            return data;
        }


        // GET api/values/5
        public IHttpActionResult Get(int id)
        {
            var data = db.patients.FirstOrDefault(b => b.id == id);
            VM_patient patient = new VM_patient();
            patient.id = data.id;
            patient.name = data.name; 
            patient.address = data.homeAddress;
            patient.phone = data.phone;
            patient.gender = data.gender;
            patient.photo = data.photo;
            patient.dateTime = data.registerDatetime;
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
