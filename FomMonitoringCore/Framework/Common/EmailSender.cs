using System.Net.Mail;

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
