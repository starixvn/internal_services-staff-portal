namespace TicketDesk.Web.Client.Controllers
{
    using Core.Enumerations;
    using Domain;
    using Domain.Infrastructure;
    using Domain.Model;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;
    using IO;
    using Localization.Controllers;
    using Models.Ticket;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    [RoutePrefix("ticket")]
    [Route("{action=index}")]
    public class TicketController : BaseController
    {
        public TicketController(TdDomainContext context)
        {
            Context = context;
        }

        private TdDomainContext Context { get; }

        [Route("change-ticket-subscription")]
        [HttpPost]
        [NapAuth(FunctionKey = FunctionKeys.TicketCenter, Policy = Constant.Policy.Read)]
        public async Task<JsonResult> ChangeTicketSubscription(int ticketId)
        {
            var userId = Context.SecurityProvider.CurrentUserId;
            var ticket = await Context.Tickets.Include(t => t.TicketSubscribers)
                .Include(t => t.TicketEvents.Select(e => e.TicketEventNotifications))
                .FirstOrDefaultAsync(t => t.TicketId == ticketId).ConfigureAwait(false);
            var subscriber =
                ticket.TicketSubscribers.FirstOrDefault(s => s.SubscriberId == Context.SecurityProvider.CurrentUserId);
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

            await Context.SaveChangesAsync().ConfigureAwait(false);
            return new JsonCamelCaseResult { Data = new { IsSubscribed = isSubscribed } };
        }

        [Route("facility-ticket-details")]
        [NapAuth(FunctionKey = FunctionKeys.TicketCenter, Policy = Constant.Policy.Read)]
        public ActionResult FacilityTicketDetails(Ticket ticket)
        {
            NewFacilityTicketViewModel viewModel = new NewFacilityTicketViewModel
            {
                Ticket = ticket
            };
            return PartialView("_FacilityTicketDetails", viewModel);
        }

        public List<CustomFieldValue> GetCustomFieldValuesForTicket(Ticket ticket)
        {
            var customFieldValues = Context.CustomFieldValues.Where(x => x.TicketId == ticket.TicketId).ToList();

            var customFieldIds = customFieldValues.Select(x => x.CustomFieldId);

            var customFields = Context.CustomFields.Where(x => customFieldIds.Contains(x.Id));

            foreach (CustomFieldValue customFieldValue in customFieldValues)
            {
                customFieldValue.CustomField = customFields.First(x => x.Id == customFieldValue.CustomFieldId);
            }

            return customFieldValues;
        }

        [NapAuth(FunctionKey = FunctionKeys.TicketCenter, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            var items = GetIndexSubMenuItems();

            return View("_SubmenuNavigation", items);
        }

        [NapAuth(FunctionKey = FunctionKeys.NewTicket, Policy = Constant.Policy.Read)]
        [Route("new-facility")]
        public async Task<ActionResult> NewFacilityTicket()
        {
            NewFacilityTicketViewModel model = new NewFacilityTicketViewModel
            {
                TempId = Guid.NewGuid(),
                Ticket = new Ticket()
            };

            var availableCustomField = GetCustomFieldsForTicketCategory(TicketCategoryTypes.Facility);

            model.RequestType = new CustomFieldValue
            {
                CustomField = availableCustomField.First(x => x.Name == "Request Type")
            };

            model.Items = new List<Item>();

            Item item = new Item
            {
                ItemName = new CustomFieldValue
                {
                    CustomField = availableCustomField.First(x => x.Name == "Item")
                },
                Reason = new CustomFieldValue
                {
                    CustomField = availableCustomField.First(x => x.Name == "Reason for Repair/Improvement Request")
                }
            };

            model.Items.Add(item);

            model.Ticket.Owner = Context.SecurityProvider.CurrentUserId;
            model.Ticket.IsHtml = Context.TicketDeskSettings.ClientSettings.GetDefaultTextEditorType() == "summernote";

            await SetProjectInfoForModelAsync(model.Ticket).ConfigureAwait(false);

            return View("NewFacilityTicket", model);
        }

        [NapAuth(FunctionKey = FunctionKeys.NewTicket, Policy = Constant.Policy.Read)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateOnlyIncomingValues]
        [ValidateInput(false)]
        [Route("new-facility")]
        public async Task<ActionResult> NewFacilityTicket(NewFacilityTicketViewModel viewModel)
        {
            if (viewModel.Ticket.IsHtml)
            {
                viewModel.Ticket.Details = viewModel.Ticket.Details.StripHtmlWhenEmpty();
                if (string.IsNullOrEmpty(viewModel.Ticket.Details))
                {
                    ModelState.AddModelError("Details", Strings.RequiredField);
                }
            }

            // TODO: Replace with actual project ID represent for shop id here
            viewModel.Ticket.Project = Context.Projects.Find(1);
            viewModel.Ticket.CategoryId = (int)TicketCategoryTypes.Facility;
            
            if (ModelState.IsValid && await CreateTicketAsync(viewModel.Ticket, viewModel.TempId).ConfigureAwait(false))
            {
                await AddTicketCustomField(viewModel).ConfigureAwait(false);

                return Context.SecurityProvider.IsReadTicketCenter
                    ? RedirectToAction("Index", "TicketCenter", new { id = viewModel.Ticket.TicketId })
                    : RedirectToAction("Index", "Home");
            }
            
            await SetProjectInfoForModelAsync(viewModel.Ticket).ConfigureAwait(false);
            return View(viewModel);
        }

        [NapAuth(FunctionKey = FunctionKeys.NewTicket, Policy = Constant.Policy.Read)]
        [Route("new-feedback")]
        public ActionResult NewFeedbackTicket()
        {
            var availableCustomField = GetCustomFieldsForTicketCategory(TicketCategoryTypes.Feedback);

            NewFeedbackTicketViewModel viewModel = new NewFeedbackTicketViewModel();

            viewModel.TempId = Guid.NewGuid();
            viewModel.FeedbackBy.CustomField = availableCustomField.First(x => x.Name == "Feedback by");
            ////viewModel.Subject.CustomField = availableCustomField.First(x => x.Name == "Subject");
            viewModel.CustomerName.CustomField = availableCustomField.First(x => x.Name == "Customer Name");
            viewModel.Feedback.CustomField = availableCustomField.First(x => x.Name == "Feedback");

            return View("NewFeedbackTicket", viewModel);
        }

        [NapAuth(FunctionKey = FunctionKeys.NewTicket, Policy = Constant.Policy.Read)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateOnlyIncomingValues]
        [ValidateInput(false)]
        [Route("new-feedback")]
        public async Task<ActionResult> NewFeedbackTicket(NewFeedbackTicketViewModel viewModel)
        {
            if (viewModel.Ticket.IsHtml)
            {
                viewModel.Ticket.Details = viewModel.Ticket.Details.StripHtmlWhenEmpty();
                if (string.IsNullOrEmpty(viewModel.Ticket.Details))
                {
                    ModelState.AddModelError("Details", Strings.RequiredField);
                }
            }

            // TODO: Replace with actual project ID represent for shop id here
            viewModel.Ticket.Project = Context.Projects.Find(1);
            viewModel.Ticket.CategoryId = (int)TicketCategoryTypes.Feedback;
            
            // TODO: Remove guid in CreateTicketAsync
            if (ModelState.IsValid && await CreateTicketAsync(viewModel.Ticket, viewModel.TempId).ConfigureAwait(false))
            {
                AddCustomFieldValue(
                    null,
                    viewModel.FeedbackBy.CustomFieldId,
                    viewModel.FeedbackBy.Value,
                    viewModel.Ticket.TicketId);

                ////AddCustomFieldValue(
                ////    null,
                ////    viewModel.Subject.CustomFieldId,
                ////    viewModel.Subject.Value,
                ////    viewModel.Ticket.TicketId);

                AddCustomFieldValue(
                    null,
                    viewModel.CustomerName.CustomFieldId,
                    viewModel.CustomerName.Value,
                    viewModel.Ticket.TicketId);

                AddCustomFieldValue(
                    null,
                    viewModel.Feedback.CustomFieldId,
                    viewModel.Feedback.Value,
                    viewModel.Ticket.TicketId);

                // Save changes to ticket's custom field
                await Context.SaveChangesAsync().ConfigureAwait(false);

                return Context.SecurityProvider.IsReadTicketCenter
                    ? RedirectToAction("Index", "TicketCenter", new { id = viewModel.Ticket.TicketId })
                    : RedirectToAction("Index", "Home");
            }

            await SetProjectInfoForModelAsync(viewModel.Ticket).ConfigureAwait(false);
            return View("NewFeedbackTicket", viewModel);
        }

        [NapAuth(FunctionKey = FunctionKeys.NewTicket, Policy = Constant.Policy.Read)]
        [Route("new-hardware")]
        public ActionResult NewHardwareTicket()
        {
            //var availableCustomField = GetCustomFieldsForTicketCategory(TicketCategoryTypes.Feedback);

            NewHardwareTicketViewModel viewModel = new NewHardwareTicketViewModel();
            
            return View("NewHardwareTicket", viewModel);
        }

        [Route("ticket-details")]
        [NapAuth(FunctionKey = FunctionKeys.TicketCenter, Policy = Constant.Policy.Read)]
        public ActionResult TicketDetails(int ticketId, int? categoryId)
        {
            ////if (categoryId == (int)TicketCategoryTypes.Feedback)
            ////{
            ////    return TicketFeedbackDetails(ticketId);
            ////}

            // WARNING! This is also used as a child action and cannot be made async in MVC 5
            ////ViewBag.DisplayProjects = Context.Projects.Any();

            var ticket = Context.Tickets.Find(ticketId);

            SingleTicketViewModel singleTicketViewModel = new SingleTicketViewModel
            {
                Ticket = ticket,
                CustomFieldValues = GetCustomFieldValuesForTicket(ticket)
            };

            return PartialView("_TicketDetails", singleTicketViewModel);

            ////if (ticket == null)
            ////{
            ////return PartialView("_TicketDetails", null);
            ////}

            ////switch (ticket.CategoryId)
            ////{
            ////    case (int)TicketCategoryTypes.Feedback:
            ////        return TicketFeedbackDetails(ticket);
            ////    case (int)TicketCategoryTypes.Facility:
            ////        return FacilityTicketDetails(ticket);
            ////    default:
            ////        return PartialView("_TicketDetails", ticket);
            ////}
        }

        [Route("ticket-events")]
        [NapAuth(FunctionKey = FunctionKeys.TicketCenter, Policy = Constant.Policy.Read)]
        public ActionResult TicketEvents(int ticketId)
        {
            // WARNING! This is also used as a child action and cannot be made async in MVC 5
            var ticket = Context.Tickets.Find(ticketId);
            return PartialView("_TicketEvents", ticket?.TicketEvents);
        }

        [Route("ticket-feedback-details")]
        [NapAuth(FunctionKey = FunctionKeys.TicketCenter, Policy = Constant.Policy.Read)]
        public ActionResult TicketFeedbackDetails(Ticket ticket)
        {
            // TODO: Set project id as shop id
            ticket.Project = Context.Projects.First(x => x.ProjectId == 1);

            var customFieldValues = GetCustomFieldValuesForTicket(ticket);

            var viewModel = new NewFeedbackTicketViewModel();
            viewModel.FeedbackBy = customFieldValues.First(x => x.CustomField.Name == "Feedback by");
            ////viewModel.Subject = customFieldValues.First(x => x.CustomField.Name == "Subject");
            viewModel.CustomerName = customFieldValues.First(x => x.CustomField.Name == "Customer Name");
            viewModel.Feedback = customFieldValues.First(x => x.CustomField.Name == "Feedback");
            
            return PartialView("_FeedbackTicketDetails", viewModel);
        }

        [Route("ticket-files")]
        [NapAuth(FunctionKey = FunctionKeys.TicketCenter, Policy = Constant.Policy.Read)]
        public ActionResult TicketFiles(int ticketId)
        {
            // WARNING! This is also used as a child action and cannot be made async in MVC 5
            var attachments =
                TicketDeskFileStore.ListAttachmentInfo(ticketId.ToString(CultureInfo.InvariantCulture), false);
            ViewBag.TicketId = ticketId;
            return PartialView("_TicketFiles", attachments);
        }

        private void AddCustomFieldValue(string groupId, int customFieldId, string value, int ticketId)
        {
            Context.CustomFieldValues.Add(new CustomFieldValue
            {
                GroupId = groupId,
                CustomFieldId = customFieldId,
                Value = value,
                TicketId = ticketId
            });
        }

        private async Task AddTicketCustomField(NewFacilityTicketViewModel viewModel)
        {
            AddCustomFieldValue(
                null,
                viewModel.RequestType.CustomFieldId,
                viewModel.RequestType.Value,
                viewModel.Ticket.TicketId);

            // add item to database
            foreach (Item item in viewModel.Items)
            {
                AddCustomFieldValue(
                    item.Index,
                    item.ItemName.CustomFieldId,
                    item.ItemName.Value,
                    viewModel.Ticket.TicketId);

                AddCustomFieldValue(
                    item.Index,
                    item.Reason.CustomFieldId,
                    item.Reason.Value,
                    viewModel.Ticket.TicketId);
            }

            // Save changes to ticket's custom field
            await Context.SaveChangesAsync().ConfigureAwait(false);
        }

        private async Task<bool> CreateTicketAsync(Ticket ticket, Guid tempId)
        {
            Context.Tickets.Add(ticket);
            await Context.SaveChangesAsync().ConfigureAwait(false);
            ticket.CommitPendingAttachments(tempId);

            return ticket.TicketId != default(int);
        }

        private IQueryable<CustomField> GetCustomFieldsForTicketCategory(TicketCategoryTypes type)
        {
            var customFieldIds = Context.TicketCategoryCustomField
                .Where(x => x.CategoryId == (int)type)
                .Select(x => x.CustomFieldId);

            var availableCustomField = Context.CustomFields.Where(x => customFieldIds.Contains(x.Id));

            foreach (CustomField customField in availableCustomField)
            {
                customField.CustomFieldValues =
                    Context.CustomFieldValues.Where(x => x.CustomFieldId == customField.Id).ToList();
            }

            return availableCustomField;
        }

        // [NapAuth(FunctionKey = FunctionKeys.NewTicket, Policy = Constant.Policy.Read)]
        private async Task SetProjectInfoForModelAsync(Ticket ticket)
        {
            if (ticket.ProjectId == default(int))
            {
                var projects = await Context.Projects.Select(s => s.ProjectId).ToListAsync().ConfigureAwait(false);
                var isMulti = projects.Count > 1;
                ViewBag.IsMultiProject = isMulti;

                // set to first project if only one project exists, otherwise use user's selected project
                ticket.ProjectId = isMulti
                    ? await Context.UserSettingsManager.GetUserSelectedProjectIdAsync(Context).ConfigureAwait(false)
                    : projects.FirstOrDefault();
            }
        }
    }
}