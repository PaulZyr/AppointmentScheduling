using Mailjet.Client;
using Mailjet.Client.Resources;
using Microsoft.AspNetCore.Identity.UI.Services;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace AppointmentScheduling.Utility
{
    public class EmailSender : IEmailSender
    {
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            MailjetClient client = new MailjetClient(
                "e5495ef73b8dc746937b8c48e501e117", 
                "cbe2a8adb1332db5832ea83ea0f7cd26") 
                { };
            MailjetRequest request = new MailjetRequest
                {
                    Resource = Send.Resource,
                }
                .Property(Send.FromEmail, "paulzyr@outlook.com")
                .Property(Send.FromName, "Appointment Scheduler")
                .Property(Send.Subject, subject)
                .Property(Send.HtmlPart, htmlMessage)
                .Property(Send.Recipients, new JArray {
                    new JObject {
                        {"Email", "pastpav@gmail.com"} //email
                    }
                });
            MailjetResponse response = await client.PostAsync(request);
        }
    }
}
