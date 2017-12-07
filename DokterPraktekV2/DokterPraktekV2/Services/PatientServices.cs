using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DokterPraktekV2.ControllerApi;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;
using DokterPraktekV2.Models;
using DokterPraktekV2.Controllers;
using Microsoft.AspNet.Identity;
using DokterPraktekV2;
using Microsoft.Ajax.Utilities;

namespace DokterPraktekV2.Services
{
    public class PatientServices
    {
        private DokterPraktekEntities1 db = new DokterPraktekEntities1();
        private string baseUrl = "http://localhost:7188/";


        /*get all patient from doctorId*/
        public List<VM_patient> allDoctorPatient(int id)
        {
            List<VM_patient> dataPatient = new List<VM_patient>();
            var client = new HttpClient();
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage Res = client.GetAsync("api/DoctorPatient?doctorId=" + id).Result;
            if (Res.IsSuccessStatusCode)
            {
                var patientResponse = Res.Content.ReadAsStringAsync().Result;
                dataPatient = JsonConvert.DeserializeObject<List<VM_patient>>(patientResponse);
            }
            return dataPatient;
        }

        /*seacrh patient from name and doctor Id*/
        public List<VM_patient> searchPatientFromNameAndIdDoctor(string name, int doctorId)
        {
            var data = db.histories.DistinctBy(a => a.patientId)
                .Where(z => z.doctorId == doctorId && z.patient.name.Contains(name) 
                || z.doctorId == doctorId && z.patient.name == name)
                .Select(e => new VM_patient
            {
                id = e.id,
                name = e.patient.name,
                address = e.patient.homeAddress,
                phone = e.patient.phone,
                gender = e.patient.gender,
                photo = e.patient.photo,
                dateTime = e.patient.registerDatetime
            }).ToList();
            return data;
        }
              
        /* get all patient index */
        public List<VM_patient> allPatient()
        {
            List<VM_patient> dataPatient = new List<VM_patient>();
            var client = new HttpClient();
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage Res = client.GetAsync("api/Patient").Result;
            if (Res.IsSuccessStatusCode)
            {
                var patientResponse = Res.Content.ReadAsStringAsync().Result;
                dataPatient = JsonConvert.DeserializeObject<List<VM_patient>>(patientResponse);
            }
            return dataPatient;
        }

        /* get patient detail from table patient column id*/
        public VM_patient patientDetail(int ? id)
        {
            VM_patient dataPatient = new VM_patient();
            var client = new HttpClient();
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage Res = client.GetAsync("api/patient/" + id).Result;
            if (Res.IsSuccessStatusCode)
            {
                var patientResponse = Res.Content.ReadAsStringAsync().Result;
                dataPatient = JsonConvert.DeserializeObject<VM_patient>(patientResponse);
            }
            return dataPatient;
        }

        /* get all patient history from patient id */
        public List<VM_history> allPatientHistory(int ? id, int ? doctorID)
        {
            List<VM_history> dataHistory = new List<VM_history>();
            var client = new HttpClient();
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage Res = client.GetAsync("/api/HistoryPatient/" + id +"?doctorID=" + doctorID).Result;
            if (Res.IsSuccessStatusCode)
            {
                var historyResponse = Res.Content.ReadAsStringAsync().Result;
                dataHistory = JsonConvert.DeserializeObject<List<VM_history>>(historyResponse);
            }
            return dataHistory;
        }


        /*get detail history from history id*/
        public VM_history detailHistory(int ? id, int ? doctorID)
        {
            VM_history dataHistory = new VM_history();
            var client = new HttpClient();
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage Res = client.GetAsync("api/History/" + id + "?doctorID=" + doctorID).Result;
            if (Res.IsSuccessStatusCode)
            {
                var historyResponse = Res.Content.ReadAsStringAsync().Result;
                dataHistory = JsonConvert.DeserializeObject<VM_history>(historyResponse);
            }
            return dataHistory;
        }

        /*get all medicine patient from history id*/
        public List<VM_patientMedicine> getAllMedicinePatient(int ? id)
        {
            List<VM_patientMedicine> dataPatientMedicine = new List<VM_patientMedicine>();
            var client = new HttpClient();
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage Res = client.GetAsync("api/PatientMedicine/" + id).Result;
            if (Res.IsSuccessStatusCode)
            {
                var medicineResponse = Res.Content.ReadAsStringAsync().Result;
                dataPatientMedicine = JsonConvert.DeserializeObject<List<VM_patientMedicine>>(medicineResponse);
            }
            return dataPatientMedicine;
        }


        /*get doctor patient*/
        public List<VM_patient> searchPatientFromName(string name)
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
            }).Where(s => s.name.Contains(name)).ToList();

            return data;        
        }

        /*find medicine price patient from history id*/
        public decimal totalMedicinePatient(int ? id)
        {
            var data = db.patientMedicines.Where(a => a.historyId == id);
            var leng = data.Count();
            if(leng == 0)
            {
                return 0;
            }
            else
            {
                return data.Sum(e => e.medicinePrice * e.quantity);
            }
        }

        public patient CreatePatient(VM_schedules data)
        {
            var dataPatient = new patient()
            {
                name = data.name,
                homeAddress = data.homeAddress,
                phone = data.phone,
                gender = data.gender
            };

            db.patients.Add(dataPatient);
            db.SaveChanges();
            return dataPatient;
        }
        
    }
}