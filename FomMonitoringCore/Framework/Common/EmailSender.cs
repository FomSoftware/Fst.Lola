using FomMonitoringCore.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace FomMonitoringCore.Framework.Common
{
    public class EmailSender
    {
        public static void SendEmail(MailMessage message)
        {
            SmtpClient mSmtpClient = new SmtpClient();          
            mSmtpClient.Send(message);
            ;
        }

    }
}
