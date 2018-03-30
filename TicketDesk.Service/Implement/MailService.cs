namespace TicketDesk.Service.Implement
{
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Domain.Model;
    using Fanex.MailDeliver.MailClient.API;
    using Fanex.MailDeliver.Model;
    using Interface;
    using IO;
    using IO.FileStorage;

    public class MailEmptyController : Controller
    {
    }

    public class MailService : EntityFrameworkBaseService, IMailService
    {
        private readonly ICustomFieldService customFieldService;
        private readonly ITicketEventService ticketEventService;

        public MailService(ICustomFieldService customFieldService, ITicketEventService ticketEventService)
        {
            this.customFieldService = customFieldService;
            this.ticketEventService = ticketEventService;
        }

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

            var createdBy = context.SecurityProvider.NapContextHelper.UserService.GetById(int.Parse(ticket.CreatedBy));
            var lastUpdateBy = context.SecurityProvider.NapContextHelper.UserService.GetById(int.Parse(ticket.LastUpdateBy));

            var singleTicketViewModel = new SingleTicketViewModel
            {
                Ticket = ticket,
                CustomFieldValues = customFieldService.GetCustomFieldValuesForTicket(ticket),
                ShopName = context.Shops.Find(ticket.ShopId)?.ShopName,
                CreatedByUsername = createdBy.Username,
                UpdatedByUsername = lastUpdateBy.Username,
                Attachments = TicketDeskFileStore.ListAttachmentInfo(ticket.TicketId.ToString(), false),
                TicketEditedMessage = ticketEventService.GetLastStatusChangeTicketEventMessage(ticket.TicketId)
            };

            var body = GetRazorViewAsString(singleTicketViewModel, "~/Views/MailTemplate/GeneralTicketDetail.cshtml");

            var mailRecord = new MailRecord();

            if (ticketCategory != null)
            {
                var mailTo = ticketCategory.MailTo;
                var mailCcTo = ticketCategory.MailCcTo;

                if (!string.IsNullOrEmpty(ticket.AdditionalMail))
                {
                    mailCcTo = mailCcTo + ";" + ticket.AdditionalMail;
                }

                if (string.IsNullOrEmpty(mailTo))
                {
                    mailTo = mailCcTo;
                    mailCcTo = null;
                }

                mailRecord = new MailRecord
                {
                    Title = ConfigurationManager.AppSettings["app_mail_title"],
                    MailTo = mailTo,
                    MailCc = mailCcTo,
                    MailBcc = ticketCategory.MailBccTo,
                    Body = body,
                    Subject = ticket.Title
                };
            }

            return mailRecord;
        }

        public void ComposeAndSendMail(Ticket ticket)
        {
            var mailRecord = ComposeMail(ticket);

            if (!string.IsNullOrEmpty(mailRecord.MailTo))
            {
                Send(mailRecord);
            }
        }

        public bool IsEmailListValid(string emails)
        {
            var result = true;

            if (!string.IsNullOrEmpty(emails))
            {
                var emailStrings = emails.Split(',', ' ', ';');

                if (emailStrings.Any(emailString => !IsEmailValid(emailString.Trim())))
                {
                    result = false;
                }
            }

            return result;
        }

        public bool IsEmailValid(string email)
        {
            return Regex.IsMatch(email, @"^[^\s@]+@[^\s@]+\.[^\s@]+$");
        }
    }
}
