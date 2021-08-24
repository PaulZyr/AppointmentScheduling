using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppointmentScheduling.Models.ViewModels;

namespace AppointmentScheduling.Services
{
    public interface IAppointmentService
    {
        public List<DoctorViewModel> GetDoctorList();
        public List<PatientViewModel> GetPatientList();
        public Task<int> AddUpdate(AppointmentViewModel model);
        public List<AppointmentViewModel> DoctorsEventById(string doctorId);
        public List<AppointmentViewModel> PatientsEventById(string patientId);
    }
}
