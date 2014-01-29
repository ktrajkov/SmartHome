
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace SmartHome.Api.Infrastructure.Abstract
{
    public interface  IEmailSender
    {
        void SendEmail(MailMessage message);
    }
}