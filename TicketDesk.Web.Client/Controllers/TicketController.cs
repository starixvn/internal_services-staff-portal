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
    using System.Data.Entity.Validation;
    using System.Data.SqlClient;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using ViewModels.Ticket;

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
        public async Task<ActionResult> New(int categoryId = 1)
        {
            NewTicketViewModel model = new NewTicketViewModel
            {
                Ticket = new Ticket()
            };

            var customFieldIds = Context.TicketCategoryCustomField.Where(x => x.CategoryId == categoryId)
                .Select(x => x.CustomFieldId);

            model.CustomFields = Context.CustomFields.Where(x => customFieldIds.Contains(x.Id));

            foreach (CustomField customField in model.CustomFields)
            {
                customField.CustomFieldValues =
                    Context.CustomFieldValues.Where(x => x.CustomFieldId == customField.Id).ToList();
            }

            model.Ticket.Owner = Context.SecurityProvider.CurrentUserId;
            model.Ticket.IsHtml = Context.TicketDeskSettings.ClientSettings.GetDefaultTextEditorType() == "summernote";

            await SetProjectInfoForModelAsync(model.Ticket);

            ViewBag.TempId = Guid.NewGuid();

            return View(model);
        }

        [NapAuth(FunctionKey = FunctionKeys.NewTicket, Policy = Constant.Policy.Read)]
        [Route("new-facility")]
        public async Task<ActionResult> NewFacilityTicket()
        {
            NewFacilityTicketViewModel model = new NewFacilityTicketViewModel
            {
                Ticket = new Ticket()
            };

            var customFieldIds = Context.TicketCategoryCustomField.Where(x => x.CategoryId == (int)TicketCategoryTypes.Facility)
                .Select(x => x.CustomFieldId);

            var avalableCustomFields = Context.CustomFields.Where(x => customFieldIds.Contains(x.Id));

            foreach (CustomField customField in avalableCustomFields)
            {
                customField.CustomFieldValues =
                    Context.CustomFieldValues.Where(x => x.CustomFieldId == customField.Id).ToList();
            }

            model.RequestType = avalableCustomFields.First(x => x.Name == "Request Type");

            model.Items = new List<Item>();
            Item item = new Item
            {
                ItemName = avalableCustomFields.First(x => x.Name == "Item"),
                Reason = avalableCustomFields.First(x => x.Name == "Reason for Repair/Improvement Request")
            };
            model.Items.Add(item);

            //item = new Item
            //{
            //    ItemName = avalableCustomFields.First(x => x.Name == "Item"),
            //    Reason = avalableCustomFields.First(x => x.Name == "Reason for Repair/Improvement Request")
            //};
            //model.Items.Add(item);

            model.Ticket.Owner = Context.SecurityProvider.CurrentUserId;
            model.Ticket.IsHtml = Context.TicketDeskSettings.ClientSettings.GetDefaultTextEditorType() == "summernote";

            await SetProjectInfoForModelAsync(model.Ticket);

            ViewBag.TempId = Guid.NewGuid();

            return View("NewFacilityTicket", model);
        }

        [NapAuth(FunctionKey = FunctionKeys.NewTicket, Policy = Constant.Policy.Read)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateOnlyIncomingValues]
        [ValidateInput(false)]
        [Route("new-facility")]
        public async Task<ActionResult> NewFacilityTicket(NewFacilityTicketViewModel viewModel, Guid tempId)
        {
            if (viewModel.Ticket.IsHtml)
            {
                viewModel.Ticket.Details = viewModel.Ticket.Details.StripHtmlWhenEmpty();
                if (string.IsNullOrEmpty(viewModel.Ticket.Details))
                {
                    ModelState.AddModelError("Details", Strings.RequiredField);
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // TODO: Replace with actual project ID represent for shop id here
                    viewModel.Ticket.Project = Context.Projects.Find(1);
                    viewModel.Ticket.CategoryId = (int)TicketCategoryTypes.Facility;

                    if (await CreateTicketAsync(viewModel.Ticket, tempId))
                    {
                        //// add item to database
                        //foreach (Item item in viewModel.Items)
                        //{
                        //    Context.CustomFieldValues.Add(new CustomFieldValue
                        //    {
                        //        GroupId = item.Index.ToString()
                        //    });
                        //}

                        if (Context.SecurityProvider.IsReadTicketCenter)
                        {
                            return RedirectToAction("Index", new { id = viewModel.Ticket.TicketId });
                        }

                        return RedirectToAction("Index", "Home");
                    }
                }
                // ReSharper disable once EmptyGeneralCatchClause
                catch (DbEntityValidationException ex)
                {
                    // TODO: catch rule exceptions? or can annotations handle this fully now?
                }
            }

            ViewBag.TempId = tempId;
            await SetProjectInfoForModelAsync(viewModel.Ticket);
            return View(viewModel);
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
        public ActionResult TicketDetails(int ticketId, int? categoryId)
        {
            ////if (categoryId == (int)TicketCategoryTypes.Feedback)
            ////{
            ////    return TicketFeedbackDetails(ticketId);
            ////}

            // WARNING! This is also used as a child action and cannot be made async in MVC 5
            ViewBag.DisplayProjects = Context.Projects.Any();
            var ticket = Context.Tickets.Find(ticketId);

            if (ticket == null)
            {
                return PartialView("_TicketDetails", null);
            }

            switch (ticket.CategoryId)
            {
                case (int)TicketCategoryTypes.Feedback:
                    return TicketFeedbackDetails(ticket);
                case (int)TicketCategoryTypes.Facility:
                    return FacilityTicketDetails(ticket);
                default:
                    return PartialView("_TicketDetails", ticket);
            }
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
            var categoryId = (int)TicketCategoryTypes.Feedback;
            var model = new Ticket
            {
                Owner = Context.SecurityProvider.CurrentUserId,
                IsHtml = Context.TicketDeskSettings.ClientSettings.GetDefaultTextEditorType() == "summernote",
                CategoryId = categoryId
            };

            await SetProjectInfoForModelAsync(model);

            SqlParameter categoryParam = new SqlParameter("@CategoryId", categoryId);
            var customFields = await Context.Database.SqlQuery<TicketFeedbackCustomField>("TicketFeedbackCustomFields_Get @CategoryId", categoryParam).ToListAsync();

            var ticketFeedbackViewModel = new TicketFeedbackViewModel();
            ticketFeedbackViewModel.Ticket = model;
            ticketFeedbackViewModel.CustomFields = customFields;
            ticketFeedbackViewModel.tempId = Guid.NewGuid();

            return View(ticketFeedbackViewModel);
        }

        [NapAuth(FunctionKey = FunctionKeys.NewTicket, Policy = Constant.Policy.Read)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateOnlyIncomingValues]
        [ValidateInput(false)]
        [Route("feedback/new")]
        public async Task<ActionResult> Feedback(TicketFeedbackViewModel model)
        {
            var ticket = model.Ticket;

            if (ModelState.IsValid)
            {
                try
                {
                    if (await CreateTicketFeedbackAsync(model))
                    {
                        return Context.SecurityProvider.IsReadTicketCenter ? RedirectToAction("Index", new { id = ticket.TicketId }) : RedirectToAction("Index", "Home");
                    }
                }
                // ReSharper disable once EmptyGeneralCatchClause
                catch (DbEntityValidationException e)
                {
                    // TODO: catch rule exceptions? or can annotations handle this fully now?
                }
            }

            await SetProjectInfoForModelAsync(ticket);
            return View(ticket);
        }

        [Route("ticket-feedback-details")]
        [NapAuth(FunctionKey = FunctionKeys.TicketCenter, Policy = Constant.Policy.Read)]
        public ActionResult TicketFeedbackDetails(int ticketId)
        {
            var categoryId = (int)TicketCategoryTypes.Feedback;
            var ticket = Context.Tickets.Find(ticketId);
            var viewModel = new TicketFeedbackViewModel
            {
                Ticket = ticket,
                CustomFields = new List<TicketFeedbackCustomField>()
            };

            var ticketCategory = Context.TicketCategories.Where(x => x.CategoryId == categoryId).First();
            var customFieldIds = Context.TicketCategoryCustomField.Where(x => x.CategoryId == categoryId)
            .Select(x => x.CustomFieldId);

            var avalableCustomFields = Context.CustomFields.Where(x => customFieldIds.Contains(x.Id));

            foreach (var customField in avalableCustomFields)
            {
                viewModel.CustomFields.Add(new TicketFeedbackCustomField
                {
                    CategoryId = categoryId,
                    CustomFieldId = customField.Id,
                    CustomFieldName = customField.Name,
                    Value = Context.CustomFieldValues.Where(x => (x.CustomFieldId == customField.Id) && (x.TicketId == ticketId)).FirstOrDefault().Value
                });
            }

            viewModel.Ticket.CategoryName = ticketCategory.Name;

            ViewBag.DisplayProjects = Context.Projects.Any();

            return PartialView("_TicketFeedbackDetails", viewModel);
        }

        [Route("ticket-feedback-details")]
        [NapAuth(FunctionKey = FunctionKeys.TicketCenter, Policy = Constant.Policy.Read)]
        public ActionResult TicketFeedbackDetails(Ticket ticket)
        {
            var categoryId = ticket.CategoryId;
            var viewModel = new TicketFeedbackViewModel
            {
                Ticket = ticket,
                CustomFields = new List<TicketFeedbackCustomField>()
            };

            var ticketCategory = Context.TicketCategories.First(x => x.CategoryId == categoryId);
            var customFieldIds = Context.TicketCategoryCustomField.Where(x => x.CategoryId == categoryId)
                .Select(x => x.CustomFieldId);

            var avalableCustomFields = Context.CustomFields.Where(x => customFieldIds.Contains(x.Id));

            foreach (var customField in avalableCustomFields)
            {
                viewModel.CustomFields.Add(new TicketFeedbackCustomField
                {
                    CategoryId = categoryId,
                    CustomFieldId = customField.Id,
                    CustomFieldName = customField.Name,
                    Value = Context.CustomFieldValues.FirstOrDefault(x => (x.CustomFieldId == customField.Id) && (x.TicketId == ticket.TicketId))?.Value
                });
            }

            viewModel.Ticket.CategoryName = ticketCategory.Name;

            ViewBag.DisplayProjects = Context.Projects.Any();

            return PartialView("_TicketFeedbackDetails", viewModel);
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

        private async Task<bool> CreateTicketFeedbackAsync(TicketFeedbackViewModel model)
        {
            Context.Tickets.Add(model.Ticket);
            Context.SaveChanges();

            foreach (var field in model.CustomFields)
            {
                var customFieldValue = new CustomFieldValue();
                customFieldValue.Value = field.Value;
                customFieldValue.CustomFieldId = field.CustomFieldId;
                customFieldValue.TicketId = model.Ticket.TicketId;
                Context.CustomFieldValues.Add(customFieldValue);
            }

            await Context.SaveChangesAsync();

            model.Ticket.CommitPendingAttachments(model.tempId);

            return model.Ticket.TicketId != default(int);
        }
    }
}