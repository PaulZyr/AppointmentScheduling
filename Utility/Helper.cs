using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AppointmentScheduling.Utility
{
    public static class Helper
    {
        public static string Admin = "Admin";
        public static string Patient = "Patient";
        public static string Doctor = "Doctor";

        public const int MaxHours = 6;

        public static List<SelectListItem> GetRolesForDropDown()
        {
            return new List<SelectListItem>
            {
                new SelectListItem {Value = Helper.Patient, Text = Helper.Patient},
                new SelectListItem {Value = Helper.Doctor, Text = Helper.Doctor},
                new SelectListItem {Value = Helper.Admin, Text = Helper.Admin}
            };
        }

        public static List<SelectListItem> GetTimeDropDown()
        {
            int minute = 30;
            List<SelectListItem> duration = new List<SelectListItem>();
            duration.Add(new SelectListItem { Value = minute.ToString(), Text = 0 + " Hr 30 min" });
            for (int i = 1; i < MaxHours; i++)
            {
                duration.Add(new SelectListItem { Value = minute.ToString(), Text = i + " Hr" });
                minute = minute + 30;
                duration.Add(new SelectListItem { Value = minute.ToString(), Text = i + " Hr 30 min" });
                minute = minute + 30;
            }
            duration.Add(new SelectListItem { Value = minute.ToString(), Text = MaxHours + " Hr" });
            return duration;
        }
    }
}
