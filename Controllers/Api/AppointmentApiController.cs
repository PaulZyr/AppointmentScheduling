using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AppointmentScheduling.Models;
using AppointmentScheduling.Models.ViewModels;
using AppointmentScheduling.Services;
using AppointmentScheduling.Utility;
using Microsoft.AspNetCore.Http;

namespace AppointmentScheduling.Controllers.Api
{
    [Route("api/Appointment")]
    [ApiController]
    public class AppointmentApiController : Controller
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _loginUserId;
        private readonly string _role;

        public AppointmentApiController(
            IAppointmentService appointmentService, 
            IHttpContextAccessor httpContextAccessor)
        {
            _appointmentService = appointmentService;
            if (httpContextAccessor.HttpContext != null)
            {
                _httpContextAccessor = httpContextAccessor;
                _loginUserId = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                _role = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);
            }
                
            
        }

        [HttpPost]
        [Route("SaveCalendarData")]
        public IActionResult SaveCalendarData(AppointmentViewModel data)
        {
            CommonResponse<int> commonResponse = new CommonResponse<int>();
            try
            {
                commonResponse.status = _appointmentService.AddUpdate(data).Result;
                if (commonResponse.status == 1)
                {
                    commonResponse.message = Helper.appointmentUpdated;
                }
                if (commonResponse.status == 2)
                {
                    commonResponse.message = Helper.appointmentAdded;
                }
            }
            catch (Exception exception)
            {
                commonResponse.message = exception.Message;
                commonResponse.status = Helper.failure_code;
            }

            return Ok(commonResponse);
        }

        [HttpGet]
        [Route("GetCalendarData")]
        public IActionResult GetCalendarData(string doctorId)
        {
            var commonResponse = new CommonResponse<List<AppointmentViewModel>>();
            try
            {
                if (_role == Helper.Patient)
                {
                    commonResponse.dataEnum = _appointmentService.PatientsEventById(_loginUserId);
                    commonResponse.status = Helper.success_code;
                }
                else if (_role == Helper.Doctor)
                {
                    commonResponse.dataEnum = _appointmentService.DoctorsEventById(_loginUserId);
                    commonResponse.status = Helper.success_code;
                }
                else
                {
                    commonResponse.dataEnum = _appointmentService.DoctorsEventById(doctorId);
                    commonResponse.status = Helper.success_code;
                }
            }
            catch (Exception exception)
            {
                commonResponse.message = exception.Message;
                commonResponse.status = Helper.failure_code;
            }

            return Ok(commonResponse);
        }

        [HttpGet]
        [Route("GetCalendarDataById/{id}")]
        public IActionResult GetCalendarDataById(int id)
        {
            var commonResponse = new CommonResponse<AppointmentViewModel>();
            try
            {
                commonResponse.dataEnum = _appointmentService.GetById(id);
                commonResponse.status = Helper.success_code;
            }
            catch (Exception exception)
            {
                commonResponse.message = exception.Message;
                commonResponse.status = Helper.failure_code;
            }

            return Ok(commonResponse);
        }

        [HttpGet]
        [Route("DeleteAppointment/{id}")]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            var commonResponse = new CommonResponse<int>();
            try
            {
                commonResponse.status = await _appointmentService.Delete(id);
                commonResponse.message =
                    commonResponse.status == 1 ? Helper.appointmentDeleted : Helper.somethingWentWrong;
            }
            catch (Exception exception)
            {
                commonResponse.message = exception.Message;
                commonResponse.status = Helper.failure_code;
            }
            return Ok(commonResponse);
        }

        [HttpGet]
        [Route("ConfirmEvent/{id}")]
        public async Task<IActionResult> ConfirmEvent(int id)
        {
            var commonResponse = new CommonResponse<int>();
            try
            {
                var result = await _appointmentService.ConfirmEvent(id);
                if (result > 0)
                {
                    commonResponse.status = Helper.success_code;
                    commonResponse.message = Helper.meetingConfirm;
                }
                else
                {
                    commonResponse.status = Helper.failure_code;
                    commonResponse.message = Helper.meetingConfirmError;
                }
            }
            catch (Exception exception)
            {
                commonResponse.message = exception.Message;
                commonResponse.status = Helper.failure_code;
            }
            return Ok(commonResponse);
        }
    }
}
