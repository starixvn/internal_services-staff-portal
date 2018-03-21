namespace TicketDesk.Service.Implement
{
    using System.Configuration;
    using Domain.Model;
    using Fanex.MailDeliver.MailClient.API;
    using Fanex.MailDeliver.Model;
    using Interface;

    public class MailService : EntityFrameworkBaseService, IMailService
    {
        public void Send(MailRecord mailRecord)
        {
            using (var client = new MailDeliver())
            {
                client.SendMail(mailRecord);
            }
        }

        public MailRecord ComposeMail(Ticket ticket)
        {
            // Get ticket category
            var ticketCategory = context.TicketCategories.Find(ticket.CategoryId);
            var mailRecord = new MailRecord();

            if (ticketCategory != null)
            {
                mailRecord = new MailRecord
                {
                    Title = ConfigurationManager.AppSettings["app_mail_title"],
                    MailTo = ticketCategory.MailTo,
                    MailCc = ticketCategory.MailCcTo,
                    MailBcc = ticketCategory.MailBccTo,
                    Body = "test",
                    Subject = "test subject"
                };
            }

            return mailRecord;
        }
    }
}
