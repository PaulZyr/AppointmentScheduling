using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
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
                commonResponse.Status = _appointmentService.AddUpdate(data).Result;
                if (commonResponse.Status == 1)
                {
                    commonResponse.Message = Helper.appointmentUpdated;
                }
                if (commonResponse.Status == 2)
                {
                    commonResponse.Message = Helper.appointmentAdded;
                }
            }
            catch (Exception exception)
            {
                commonResponse.Message = exception.Message;
                commonResponse.Status = Helper.failure_code;
            }

            return Ok(commonResponse);
        }
    }
}
