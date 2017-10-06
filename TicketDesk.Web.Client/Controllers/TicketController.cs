namespace TicketDesk.Web.Client.Controllers
{
    using Domain;
    using Domain.Infrastructure;
    using Domain.Model;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;
    using IO;
    using Localization.Controllers;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Validation;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using ViewModels.Ticket;
    using TicketDesk.Core.Enums;

    /// <summary>
    /// Class TicketController.
    /// </summary>
    [RoutePrefix("ticket")]
    [Route("{action=index}")]
    public class TicketController : BaseController
    {
        public TicketController(TdDomainContext context)
        {
            Context = context;
        }

        private TdDomainContext Context { get; }

        [NapAuth(FunctionKey = FunctionKeys.TicketCenter, Policy = Constant.Policy.Read)]
        public RedirectToRouteResult Index()
        {
            return RedirectToAction("Index", "TicketCenter");
        }

        [Route("{id:int}")]
        [NapAuth(FunctionKey = FunctionKeys.TicketCenter, Policy = Constant.Policy.Read)]
        public async Task<ActionResult> Index(int id)
        {
            var model = await Context.Tickets.Include(t => t.TicketSubscribers).FirstOrDefaultAsync(t => t.TicketId == id);
            if (model == null)
            {
                return RedirectToAction("Index", "TicketCenter");
            }

            ViewBag.IsEditorDefaultHtml = Context.TicketDeskSettings.ClientSettings.GetDefaultTextEditorType() == "summernote";

            return View(model);
        }

        [NapAuth(FunctionKey = FunctionKeys.NewTicket, Policy = Constant.Policy.Read)]
        [Route("new")]
        public async Task<ActionResult> New()
        {

            var model = new Ticket
            {
                Owner = Context.SecurityProvider.CurrentUserId,
                IsHtml = Context.TicketDeskSettings.ClientSettings.GetDefaultTextEditorType() == "summernote"
            };

            await SetProjectInfoForModelAsync(model);

            ViewBag.TempId = Guid.NewGuid();

            return View(model);
        }

        [NapAuth(FunctionKey = FunctionKeys.NewTicket, Policy = Constant.Policy.Read)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateOnlyIncomingValues]
        [ValidateInput(false)]
        [Route("new")]
        public async Task<ActionResult> New(Ticket ticket, Guid tempId)
        {
            if (ticket.IsHtml)
            {
                ticket.Details = ticket.Details.StripHtmlWhenEmpty();
                if (string.IsNullOrEmpty(ticket.Details))
                {
                    ModelState.AddModelError("Details", Strings.RequiredField);
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (await CreateTicketAsync(ticket, tempId))
                    {
                        return Context.SecurityProvider.IsReadTicketCenter ? RedirectToAction("Index", new { id = ticket.TicketId }) : RedirectToAction("Index", "Home");
                    }
                }
                // ReSharper disable once EmptyGeneralCatchClause
                catch (DbEntityValidationException)
                {
                    // TODO: catch rule exceptions? or can annotations handle this fully now?
                }
            }

            ViewBag.TempId = tempId;
            await SetProjectInfoForModelAsync(ticket);
            return View(ticket);
        }

        [Route("ticket-files")]
        [NapAuth(FunctionKey = FunctionKeys.TicketCenter, Policy = Constant.Policy.Read)]
        public ActionResult TicketFiles(int ticketId)
        {
            // WARNING! This is also used as a child action and cannot be made async in MVC 5
            var attachments = TicketDeskFileStore.ListAttachmentInfo(ticketId.ToString(CultureInfo.InvariantCulture), false);
            ViewBag.TicketId = ticketId;
            return PartialView("_TicketFiles", attachments);
        }

        [Route("ticket-events")]
        [NapAuth(FunctionKey = FunctionKeys.TicketCenter, Policy = Constant.Policy.Read)]
        public ActionResult TicketEvents(int ticketId)
        {
            // WARNING! This is also used as a child action and cannot be made async in MVC 5
            var ticket = Context.Tickets.Find(ticketId);
            return PartialView("_TicketEvents", ticket.TicketEvents);
        }

        [Route("ticket-details")]
        [NapAuth(FunctionKey = FunctionKeys.TicketCenter, Policy = Constant.Policy.Read)]
        public ActionResult TicketDetails(int ticketId)
        {
            // WARNING! This is also used as a child action and cannot be made async in MVC 5
            var ticket = Context.Tickets.Find(ticketId);
            ViewBag.DisplayProjects = Context.Projects.Any();

            return PartialView("_TicketDetails", ticket);
        }

        [Route("change-ticket-subscription")]
        [HttpPost]
        [NapAuth(FunctionKey = FunctionKeys.TicketCenter, Policy = Constant.Policy.Read)]
        public async Task<JsonResult> ChangeTicketSubscription(int ticketId)
        {
            var userId = Context.SecurityProvider.CurrentUserId;
            var ticket = await Context.Tickets.Include(t => t.TicketSubscribers)
                .Include(t => t.TicketEvents.Select(e => e.TicketEventNotifications)).FirstOrDefaultAsync(t => t.TicketId == ticketId);
            var subscriber = ticket.TicketSubscribers.FirstOrDefault(s => s.SubscriberId == Context.SecurityProvider.CurrentUserId);
            var isSubscribed = false;
            if (subscriber == null)
            {
                subscriber = new TicketSubscriber
                {
                    SubscriberId = userId,
                };
                ticket.TicketSubscribers.Add(subscriber);
                isSubscribed = true;
            }
            else
            {
                ticket.TicketSubscribers.Remove(subscriber);
            }

            await Context.SaveChangesAsync();
            return new JsonCamelCaseResult { Data = new { IsSubscribed = isSubscribed } };
        }

        // [NapAuth(FunctionKey = FunctionKeys.NewTicket, Policy = Constant.Policy.Read)]
        private async Task SetProjectInfoForModelAsync(Ticket ticket)
        {
            if (ticket.ProjectId == default(int))
            {
                var projects = await Context.Projects.Select(s => s.ProjectId).ToListAsync();
                var isMulti = projects.Count > 1;
                ViewBag.IsMultiProject = isMulti;

                // set to first project if only one project exists, otherwise use user's selected project
                ticket.ProjectId = isMulti ? await Context.UserSettingsManager.GetUserSelectedProjectIdAsync(Context) : projects.FirstOrDefault();
            }
        }

        private async Task<bool> CreateTicketAsync(Ticket ticket, Guid tempId)
        {
            Context.Tickets.Add(ticket);
            await Context.SaveChangesAsync();
            ticket.CommitPendingAttachments(tempId);

            return ticket.TicketId != default(int);
        }

        [NapAuth(FunctionKey = FunctionKeys.NewTicket, Policy = Constant.Policy.Read)]
        [Route("feedback/new")]
        public async Task<ActionResult> Feedback()
        {
            var ticketCategoryId = (int)TicketCategoryType.Feedback;
            var ticketFeedbackViewModel = new TicketFeedbackViewModel();

            var model = new Ticket
            {
                Owner = Context.SecurityProvider.CurrentUserId,
                IsHtml = Context.TicketDeskSettings.ClientSettings.GetDefaultTextEditorType() == "summernote"
            };

            model.TicketCategory = new TicketCategory(ticketCategoryId);



            var ticketCategoryCustomFields = Context.TicketCategoryCustomFields.Where(x => x.TicketCategory_CategoryId == ticketCategoryId);
            foreach (var relation in ticketCategoryCustomFields)
            {
                var customField = Context.CustomFields.Find(relation.CustomField_Id);

                model.TicketCategory.CustomFields.Add(customField);
            }

            // ticketFeedbackViewModel.TicketInfo
            ticketFeedbackViewModel.Ticket = model;

            await SetProjectInfoForModelAsync(ticketFeedbackViewModel.Ticket);


            return View(ticketFeedbackViewModel);
        }
    }
}