namespace TicketDesk.Service.Implement
{
    using System.Configuration;
    using Core.Model;
    using Fanex.MailDeliver.MailClient.API;
    using Fanex.MailDeliver.Model;
    using Interface;

    public class MailService : IMailService
    {
        public void Send(MailMessage message)
        {
            var mailItem = new MailRecord
            {
                MailTo = message.ReceiverEmail,
                MailCc = message.MailCC,
                Subject = message.Subject,
                Title = ConfigurationManager.AppSettings["app_mail_title"],
                Body = message.Body
            };

            using (var client = new MailDeliver())
            {
                client.SendMail(mailItem);
            }
        }
    }
}
