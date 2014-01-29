using SmartHome.Api.Infrastructure.Abstract;
using SmartHome.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;

namespace SmartHome.Api.Infrastructure.Concrete
{


    public class EmailSender : IEmailSender
    {
        private EmailSettings EmailSettings;
        public EmailSender(EmailSettings settings)
        {
            EmailSettings = settings;
        }
        public void SendEmail(MailMessage message)
        {
            using (var smtpClient = new SmtpClient())
            {
                smtpClient.EnableSsl = EmailSettings.UseSsl;
                smtpClient.Host = EmailSettings.ServerName;
                smtpClient.Port = EmailSettings.ServerPort;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(EmailSettings.Username,EmailSettings.Password);
                smtpClient.Send(message);
            }
        }
    }
}
        