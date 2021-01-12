using System.Linq;
using System.Net.Mail;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Service;

namespace FomMonitoringCore.Queue.Notifier
{ 
    public static class FailedJsonProcessorNotifier
    {
        public static void Notify(string objectId, string entity)
        {
            var message = new MailMessage
            {
                From = new MailAddress(ApplicationSettingService.GetWebConfigKey("EmailFromAddress")),
                Subject = "[LOLA] - Errore assorbimento JSON",
                Body =
                    $"Il JSON con id: {objectId}, entità: {entity} ha riportato un errore durante il processamento, verificare i dettagli nel file di LOG."
            };

            foreach (var mail in ApplicationSettingService.GetWebConfigKey("EmailToAddress").Split(';').ToList())
            {
                message.To.Add(mail);
            }


            EmailSender.SendEmail(message);
        }
    }
}
