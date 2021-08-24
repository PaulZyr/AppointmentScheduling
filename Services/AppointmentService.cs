using AppointmentScheduling.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppointmentScheduling.Data;
using AppointmentScheduling.Utility;
using Microsoft.AspNetCore.Identity;

namespace AppointmentScheduling.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly AppDbContext _db;

        public AppointmentService(AppDbContext db)
        {
            _db = db;
        }
        public List<DoctorViewModel> GetDoctorList()
        {
            var doctors = (from user in _db.Users
                join userRole in _db.UserRoles on user.Id equals  userRole.UserId
                join role in _db.Roles.Where(x=>x.Name==Helper.Doctor) on userRole.RoleId equals role.Id
                select new DoctorViewModel
                {
                    Id = user.Id,
                    Name = user.Name
                }).ToList();
            return doctors;
        }

        public List<PatientViewModel> GetPatientList()
        {
            var patients = (from user in _db.Users
                join userRole in _db.UserRoles on user.Id equals userRole.UserId
                join role in _db.Roles.Where(x => x.Name == Helper.Patient) on userRole.RoleId equals role.Id
                select new PatientViewModel
                {
                    Id = user.Id,
                    Name = user.Name
                }).ToList();
            return patients;
        }
    }
}
