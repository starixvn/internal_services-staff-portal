namespace TicketDesk.Web.Client.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Core.Enumerations;
    using Domain;
    using Domain.Infrastructure;
    using Domain.Model;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;
    using IO;
    using Localization.Controllers;
    using Models.Ticket;

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

            await Context.SaveChangesAsync().ConfigureAwait(false);
            return new JsonCamelCaseResult { Data = new { IsSubscribed = isSubscribed } };
        }

        [Route("facility-ticket-details")]
        [NapAuth(FunctionKey = FunctionKeys.TicketCenter, Policy = Constant.Policy.Read)]
        public ActionResult FacilityTicketDetails(Ticket ticket)
        {
            var viewModel = new NewFacilityTicketViewModel
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
            var availableCustomField = GetCustomFieldsForTicketCategory(TicketCategoryTypes.Facility);

            var viewModel = new NewFacilityTicketViewModel
            {
                TempId = Guid.NewGuid(),
                RequestType = { CustomField = availableCustomField.First(x => x.Name == "Request Type") }
            };

            // TODO: Bug can happen here
            // Mapping object from custom field to ViewModel manually
            var item = new Item
            {
                ItemName = { CustomField = availableCustomField.First(x => x.Name == "Item") },
                Reason =
                {
                    CustomField = availableCustomField.First(x => x.Name == "Reason for Repair/Improvement Request")
                }
            };

            viewModel.Items.Add(item);

            viewModel.Ticket.Owner = Context.SecurityProvider.CurrentUserId;
            viewModel.Ticket.IsHtml = Context.TicketDeskSettings.ClientSettings.GetDefaultTextEditorType() == "summernote";

            await SetProjectInfoForModelAsync(viewModel.Ticket).ConfigureAwait(false);

            return View("NewFacilityTicket", viewModel);
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

            // TODO: Re-prepare model for view
            var availableCustomField = GetCustomFieldsForTicketCategory(TicketCategoryTypes.Facility);
            viewModel.RequestType.CustomField = availableCustomField.First(x => x.Name == "Request Type");

            return View(viewModel);
        }

        [NapAuth(FunctionKey = FunctionKeys.NewTicket, Policy = Constant.Policy.Read)]
        [Route("new-feedback")]
        public async Task<ActionResult> NewFeedbackTicket()
        {
            var availableCustomField = GetCustomFieldsForTicketCategory(TicketCategoryTypes.Feedback);

            var viewModel = new NewFeedbackTicketViewModel
            {
                TempId = Guid.NewGuid(),
                FeedbackBy = { CustomField = availableCustomField.First(x => x.Name == "Feedback by") },
                CustomerName = { CustomField = availableCustomField.First(x => x.Name == "Customer Name") },
                Feedback = { CustomField = availableCustomField.First(x => x.Name == "Feedback") },
                Ticket =
                {
                    Owner = Context.SecurityProvider.CurrentUserId,
                    IsHtml = Context.TicketDeskSettings.ClientSettings.GetDefaultTextEditorType() == "summernote"
                }
            };

            await SetProjectInfoForModelAsync(viewModel.Ticket).ConfigureAwait(false);

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

            if (ModelState.IsValid && await CreateTicketAsync(viewModel.Ticket, viewModel.TempId).ConfigureAwait(false))
            {
                AddCustomFieldValue(null, viewModel.FeedbackBy.CustomFieldId, viewModel.FeedbackBy.Value, viewModel.Ticket.TicketId);
                AddCustomFieldValue(null, viewModel.CustomerName.CustomFieldId, viewModel.CustomerName.Value, viewModel.Ticket.TicketId);
                AddCustomFieldValue(null, viewModel.Feedback.CustomFieldId, viewModel.Feedback.Value, viewModel.Ticket.TicketId);

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
        [Route("new-incident")]
        public ActionResult NewIncidentTicket()
        {
            var availableCustomField = GetCustomFieldsForTicketCategory(TicketCategoryTypes.Incident);

            var viewModel = new NewIncidentTicketViewModel
            {
                TempId = Guid.NewGuid(),
                ReportBy = { CustomField = availableCustomField.First(x => x.Name == "Report By") },
                TimeOfIncident = { CustomField = availableCustomField.First(x => x.Name == "Time of incident") },
                Desciption = { CustomField = availableCustomField.First(x => x.Name == "Description") }
            };

            if (viewModel.TimeOfIncident.Value == null)
            {
                var nowDate = DateTime.Now.ToString("yyyy-MM-ddThh:mm");
                viewModel.TimeOfIncident.Value = nowDate;
            }

            return View("NewIncidentTicket", viewModel);
        }

        [NapAuth(FunctionKey = FunctionKeys.NewTicket, Policy = Constant.Policy.Read)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateOnlyIncomingValues]
        [ValidateInput(false)]
        [Route("new-incident")]
        public async Task<ActionResult> NewIncidentTicket(NewIncidentTicketViewModel viewModel)
        {
            if (viewModel.Ticket.IsHtml)
            {
                viewModel.Ticket.Details = viewModel.Ticket.Details.StripHtmlWhenEmpty();
                if (string.IsNullOrEmpty(viewModel.Ticket.Details))
                {
                    ModelState.AddModelError("Details", Strings.RequiredField);
                }
            }

            viewModel.Ticket.Project = Context.Projects.Find(1);
            viewModel.Ticket.CategoryId = (int)TicketCategoryTypes.Incident;

            if (ModelState.IsValid && await CreateTicketAsync(viewModel.Ticket, viewModel.TempId).ConfigureAwait(false))
            {
                AddCustomFieldValue(null, viewModel.ReportBy.CustomFieldId, viewModel.ReportBy.Value, viewModel.Ticket.TicketId);
                AddCustomFieldValue(null, viewModel.TimeOfIncident.CustomFieldId, viewModel.TimeOfIncident.Value, viewModel.Ticket.TicketId);
                AddCustomFieldValue(null, viewModel.Desciption.CustomFieldId,  viewModel.Desciption.Value, viewModel.Ticket.TicketId);

                await Context.SaveChangesAsync().ConfigureAwait(false);

                return Context.SecurityProvider.IsReadTicketCenter
                    ? RedirectToAction("Index", "TicketCenter", new { id = viewModel.Ticket.TicketId })
                    : RedirectToAction("Index", "Home");
            }

            await SetProjectInfoForModelAsync(viewModel.Ticket).ConfigureAwait(false);
            return View("NewIncidentTicket", viewModel);
        }

        [NapAuth(FunctionKey = FunctionKeys.NewTicket, Policy = Constant.Policy.Read)]
        [Route("new-hardware")]
        public ActionResult NewHardwareTicket()
        {
            var availableCustomField = GetCustomFieldsForTicketCategory(TicketCategoryTypes.Hardware);
            var viewModel = new NewHardwareTicketViewModel
            {
                TempId = Guid.NewGuid(),
                Ticket = { Owner = Context.SecurityProvider.CurrentUserId },
                System = { CustomField = availableCustomField.First(x => x.Name == "System") },
                Description = { CustomField = availableCustomField.First(x => x.Name == "Description") }
            };

            return View("NewHardwareTicket", viewModel);
        }

        [NapAuth(FunctionKey = FunctionKeys.NewTicket, Policy = Constant.Policy.Read)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateOnlyIncomingValues]
        [ValidateInput(false)]
        [Route("new-hardware")]
        public async Task<ActionResult> NewHardwareTicket(NewHardwareTicketViewModel viewModel)
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
            viewModel.Ticket.CategoryId = (int)TicketCategoryTypes.Hardware;

            if (ModelState.IsValid && await CreateTicketAsync(viewModel.Ticket, viewModel.TempId).ConfigureAwait(false))
            {
                AddCustomFieldValue(null, viewModel.System.CustomFieldId, viewModel.System.Value, viewModel.Ticket.TicketId);
                AddCustomFieldValue(null, viewModel.Description.CustomFieldId, viewModel.Description.Value, viewModel.Ticket.TicketId);

                // Save changes to ticket's custom field
                await Context.SaveChangesAsync().ConfigureAwait(false);

                return Context.SecurityProvider.IsReadTicketCenter
                    ? RedirectToAction("Index", "TicketCenter", new { id = viewModel.Ticket.TicketId })
                    : RedirectToAction("Index", "Home");
            }

            await SetProjectInfoForModelAsync(viewModel.Ticket).ConfigureAwait(false);

            return View(viewModel);
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

            var singleTicketViewModel = new SingleTicketViewModel
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

            var viewModel = new NewFeedbackTicketViewModel
            {
                FeedbackBy = customFieldValues.First(x => x.CustomField.Name == "Feedback by"),
                CustomerName = customFieldValues.First(x => x.CustomField.Name == "Customer Name"),
                Feedback = customFieldValues.First(x => x.CustomField.Name == "Feedback")
            };

            return PartialView("_FeedbackTicketDetails", viewModel);
        }

        [Route("ticket-hardware-details")]
        [NapAuth(FunctionKey = FunctionKeys.TicketCenter, Policy = Constant.Policy.Read)]
        public ActionResult TicketHardwareDetails(Ticket ticket)
        {
            ticket.Project = Context.Projects.First(x => x.ProjectId == 1);
            var customFieldValues = GetCustomFieldValuesForTicket(ticket);
            var viewModel = new NewHardwareTicketViewModel
            {
                System = customFieldValues.First(x => x.CustomField.Name == "System"),
                Description = customFieldValues.First(x => x.CustomField.Name == "Description")
            };

            return PartialView("_HardwareTicketDetails", viewModel);
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
            AddCustomFieldValue(null, viewModel.RequestType.CustomFieldId, viewModel.RequestType.Value, viewModel.Ticket.TicketId);

            // add item to database
            foreach (Item item in viewModel.Items)
            {
                AddCustomFieldValue(item.Index, item.ItemName.CustomFieldId, item.ItemName.Value, viewModel.Ticket.TicketId);
                AddCustomFieldValue(item.Index, item.Reason.CustomFieldId, item.Reason.Value, viewModel.Ticket.TicketId);
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