namespace TicketDesk.Service.Implement
{
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Web.Mvc;
    using Domain.Model;
    using Fanex.MailDeliver.MailClient.API;
    using Fanex.MailDeliver.Model;
    using Interface;

    public class MailService : EntityFrameworkBaseService, IMailService
    {
        ////public string RenderRazorViewToString(string viewName, object model)
        ////{
        ////    ViewData.Model = model;

        ////    using (var sw = new StringWriter())
        ////    {
        ////        var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext,
        ////            viewName);

        ////        var viewContext = new ViewContext(ControllerContext, viewResult.View,
        ////            ViewData, TempData, sw);
        ////        viewResult.View.Render(viewContext, sw);
        ////        viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
        ////        return sw.GetStringBuilder().ToString();
        ////    }
        ////}

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

            ////var customFields = context.CustomFieldValues.Where(x => x.TicketId == ticket.TicketId);

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
                    Subject = ticket.Title
                };
            }

            return mailRecord;
        }
    }
}
