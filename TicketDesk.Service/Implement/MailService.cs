namespace TicketDesk.Service.Implement
{
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Domain.Model;
    using Fanex.MailDeliver.MailClient.API;
    using Fanex.MailDeliver.Model;
    using Interface;

    public class MailEmptyController : Controller
    {
    }

    public class MailService : EntityFrameworkBaseService, IMailService
    {
        public static string GetRazorViewAsString(object model, string filePath)
        {
            var st = new StringWriter();
            var context = new HttpContextWrapper(HttpContext.Current);
            var routeData = new RouteData();
            var controllerContext = new ControllerContext(new RequestContext(context, routeData), new MailEmptyController());
            var razor = new RazorView(controllerContext, filePath, null, false, null);
            razor.Render(new ViewContext(controllerContext, razor, new ViewDataDictionary(model), new TempDataDictionary(), st), st);
            return st.ToString();
        }

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

            var customFieldValues = context.CustomFieldValues.Where(x => x.TicketId == ticket.TicketId);

            var singleTicketViewModel = new SingleTicketViewModel
            {
                Ticket = ticket,
                CustomFieldValues = customFieldValues
            };

            var body = GetRazorViewAsString(singleTicketViewModel, "~/Views/MailTemplate/GeneralTicketDetail.cshtml");

            var mailRecord = new MailRecord();

            if (ticketCategory != null)
            {
                mailRecord = new MailRecord
                {
                    Title = ConfigurationManager.AppSettings["app_mail_title"],
                    MailTo = ticketCategory.MailTo,
                    MailCc = ticketCategory.MailCcTo,
                    MailBcc = ticketCategory.MailBccTo,
                    Body = body,
                    Subject = ticket.Title
                };
            }

            return mailRecord;
        }
    }
}
