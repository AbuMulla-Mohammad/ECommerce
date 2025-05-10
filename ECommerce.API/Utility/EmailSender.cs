using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net.Mail;
using System.Net;

namespace ECommerce.API.Utility
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            var senderEmail = Environment.GetEnvironmentVariable("EMAIL_USER");
            var sebderPassword = Environment.GetEnvironmentVariable("EMAIL_PASS");
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,// use your own credentials not gmail credentials
                Credentials = new NetworkCredential(senderEmail,sebderPassword)
            };

            return client.SendMailAsync(
                new MailMessage(from: "m2872001@gmail.com",
                                to: email,
                                subject,
                                message
                                )
                {
                    IsBodyHtml= true,// this will allow you to send html content
                }
                );
        }
    }
}
