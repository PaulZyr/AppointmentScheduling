using AppointmentScheduling.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppointmentScheduling.Data;
using AppointmentScheduling.Models;
using AppointmentScheduling.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentScheduling.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly AppDbContext _db;
        private readonly IEmailSender _emailSender;

        public AppointmentService(
            AppDbContext db, 
            IEmailSender emailSender)
        {
            _db = db;
            _emailSender = emailSender;
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
            var endDate = DateTime.Parse(model.StartDate, System.Globalization.CultureInfo.InvariantCulture)
                .AddMinutes(Convert.ToDouble(model.Duration));
            var patient = _db.Users.FirstOrDefault(x => x.Id == model.PatientId);
            var doctor = _db.Users.FirstOrDefault(x => x.Id == model.DoctorId);

            if (doctor != null && patient != null)
            {
                if (model.Id > 0)
                {
                    // update
                    var appointment = _db.Appointments.FirstOrDefault(x => x.Id == model.Id);
                    if (appointment != null)
                    {
                        appointment.Title = model.Title;
                        appointment.Description = model.Description;
                        appointment.StartDate = startDate;
                        appointment.EndDate = endDate;
                        appointment.Duration = model.Duration;
                        appointment.DoctorId = model.DoctorId;
                        appointment.PatientId = model.PatientId;
                        appointment.IsDoctorApproved = model.IsDoctorApproved;
                        appointment.AdminId = model.AdminId;
                    }

                    await _db.SaveChangesAsync();
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
                    await _emailSender.SendEmailAsync(
                        doctor.Email,
                        "Appointment created",
                        $"You have a new appointment in pending status with {patient.Name}");
                    await _emailSender.SendEmailAsync(
                        patient.Email,
                        "Appointment created",
                        $"You have a new appointment in pending status with {doctor.Name}");
                    _db.Appointments.Add(appointment);
                    await _db.SaveChangesAsync();
                    return 2;
                }
            }
            else
            {
                return -1;
            }
        }

        public List<AppointmentViewModel> DoctorsEventById(string doctorId)
        {
            return _db.Appointments.Where(x => x.DoctorId == doctorId).ToList().Select(c => new AppointmentViewModel()
            {
                Id = c.Id,
                Description = c.Description,
                StartDate = c.StartDate.ToString("yyyy-MM-dd HH:mm:ss"),
                EndDate = c.EndDate.ToString("yyyy-MM-dd HH:mm:ss"),
                Title = c.Title,
                Duration = c.Duration,
                IsDoctorApproved = c.IsDoctorApproved
            }).ToList();
        }

        public List<AppointmentViewModel> PatientsEventById(string patientId)
        {
            return _db.Appointments.Where(x => x.PatientId == patientId).ToList().Select(c => new AppointmentViewModel()
            {
                Id = c.Id,
                Description = c.Description,
                StartDate = c.StartDate.ToString("yyyy-MM-dd HH:mm:ss"),
                EndDate = c.EndDate.ToString("yyyy-MM-dd HH:mm:ss"),
                Title = c.Title,
                Duration = c.Duration,
                IsDoctorApproved = c.IsDoctorApproved
            }).ToList();
        }

        public AppointmentViewModel GetById(int id)
        {
            return _db.Appointments.Where(x => x.Id == id).ToList().Select(c => new AppointmentViewModel()
            {
                Id = c.Id,
                Description = c.Description,
                StartDate = c.StartDate.ToString("yyyy-MM-dd HH:mm:ss"),
                EndDate = c.EndDate.ToString("yyyy-MM-dd HH:mm:ss"),
                Title = c.Title,
                Duration = c.Duration,
                IsDoctorApproved = c.IsDoctorApproved,
                PatientId = c.PatientId,
                DoctorId = c.DoctorId,
                PatientName = _db.Users.Where(x=>x.Id==c.PatientId).Select(x=>x.Name).FirstOrDefault(),
                DoctorName = _db.Users.Where(x=>x.Id==c.DoctorId).Select(x=>x.Name).FirstOrDefault()
            }).SingleOrDefault();
        }

        public async Task<int> Delete(int id)
        {
            var appointment = _db.Appointments.FirstOrDefault(x => x.Id == id);
            if (appointment != null)
            {
                _db.Appointments.Remove(appointment);
                return await _db.SaveChangesAsync();
            }

            return 0;
        }

        public async Task<int> ConfirmEvent(int id)
        {
            var appointment = _db.Appointments.FirstOrDefault(x => x.Id == id);
            if (appointment != null)
            {
                appointment.IsDoctorApproved = true;
                return await _db.SaveChangesAsync();
            }

            return 0;
        }
    }
}
