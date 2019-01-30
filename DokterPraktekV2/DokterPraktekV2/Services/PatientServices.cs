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
        private DokterPraktekEntities db = new DokterPraktekEntities();
        private string baseUrl = "http://localhost:7188/";


        /*get all patient from doctorId*/
        public List<VM_patient> allDoctorPatient(string id)
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
            var data = db.MedicalHistories.DistinctBy(a => a.PatientID)
                .Where(z => z.DoctorID == doctorId.ToString() && z.Patient.Name.Contains(name) 
                || z.DoctorID == doctorId.ToString() && z.Patient.Name == name)
                .Select(e => new VM_patient
            {
                id = e.ID,
                name = e.Patient.Name,
                address = e.Patient.Address,
                phone = e.Patient.PhoneNumber,
                gender = e.Patient.Gender,
                photo = e.Patient.Photo,
                dateTime = e.Patient.CreatedDate
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
        public List<VM_history> allPatientHistory(int ? id, string doctorID)
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
        public VM_history detailHistory(int ? id, string doctorID)
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
            var data = db.Patients.Select(e => new VM_patient
            {
                id = e.ID,
                name = e.Name,
                address = e.Address,
                phone = e.PhoneNumber,
                gender = e.Gender,
                photo = e.Photo,
                dateTime = e.CreatedDate
            }).Where(s => s.name.Contains(name)).ToList();

            return data;        
        }

        /*find medicine price patient from history id*/
        public decimal totalMedicinePatient(int ? id)
        {
            var data = db.PatientMedicines.Where(a => a.MedicalHistoryID == id);
            var leng = data.Count();
            if(leng == 0)
            {
                return 0;
            }
            else
            {
                return data.Sum(e => e.MedicalPrice * e.Quantity);
            }
        }

        public Patient CreatePatient(VM_schedules data)
        {
            var dataPatient = new Patient()
            {
                Name = data.name,
                Address = data.homeAddress,
                PhoneNumber = data.phone,
                Gender = data.gender
            };

            db.Patients.Add(dataPatient);
            db.SaveChanges();
            return dataPatient;
        }
        
    }
}