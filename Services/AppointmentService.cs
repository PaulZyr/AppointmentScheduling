using AppointmentScheduling.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppointmentScheduling.Data;
using AppointmentScheduling.Models;
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

        public async Task<int> AddUpdate(AppointmentViewModel model)
        {
            var startDate = DateTime.Parse(model.StartDate, System.Globalization.CultureInfo.InvariantCulture);
            var endDate = DateTime.Parse(model.StartDate, System.Globalization.CultureInfo.InvariantCulture).AddMinutes(Convert.ToDouble(model.Duration));

            if (model != null && model.Id > 0)
            {
                // update
                return 1;
            }
            else
            {
                //create
                var appointment = new Appointment()
                {
                    Title = model.Title,
                    Description = model.Description,
                    StartDate = startDate,
                    EndDate = endDate,
                    Duration = model.Duration,
                    DoctorId = model.DoctorId,
                    PatientId = model.PatientId,
                    IsDoctorApproved = false,
                    AdminId = model.AdminId
                };
                _db.Appointments.Add(appointment);
                await _db.SaveChangesAsync();
                return 2;
            }
        }
    }
}
