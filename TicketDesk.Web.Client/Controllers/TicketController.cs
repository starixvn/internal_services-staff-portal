namespace TicketDesk.Web.Client.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Web.Mvc;
    using Core.Enumerations;
    using Domain.Infrastructure;
    using Domain.Model;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;
    using IO;
    using Localization.Client;
    using Localization.Controllers;
    using Models.Ticket;
    using Service.Interface;

    public class TicketController : BaseController
    {
        private readonly ITicketService ticketService;
        private readonly ICustomFieldService customFieldService;

        public TicketController(ITicketService ticketService, ICustomFieldService customFieldService)
        {
            this.ticketService = ticketService;
            this.customFieldService = customFieldService;
        }

        [HttpPost]
        [NapAuth(FunctionKey = FunctionKeys.TicketCenter, Policy = Constant.Policy.Read)]
        public JsonResult ChangeTicketSubscription(int ticketId)
        {
            var isSubscribed = ticketService.UpdateTicketSubscription(ticketId, CurrentSession.UserId.ToString());
            return new JsonCamelCaseResult { Data = new { IsSubscribed = isSubscribed } };
        }

        [NapAuth(FunctionKey = FunctionKeys.TicketCenter, Policy = Constant.Policy.Read)]
        public ActionResult FacilityTicketDetails(Ticket ticket)
        {
            var viewModel = new NewFacilityTicketViewModel
            {
                Ticket = ticket
            };

            return PartialView("_FacilityTicketDetails", viewModel);
        }

        [NapAuth(FunctionKey = FunctionKeys.TicketCenter, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            ViewBag.Title = Resources.Views_Ticket_Index_NewTicket;
            var items = GetIndexSubMenuItems();

            return View("_SubmenuNavigation", items);
        }

        [NapAuth(FunctionKey = FunctionKeys.NewTicket, Policy = Constant.Policy.Read)]
        public ActionResult NewFacilityTicket()
        {
            var availableCustomField = customFieldService.GetCustomFieldsForTicketCategory(TicketCategoryTypes.Facility);

            var viewModel = new NewFacilityTicketViewModel
            {
                TempId = Guid.NewGuid(),
                RequestType = { CustomField = availableCustomField.First(x => x.Name == "Request Type") }
            };

            // TODO: Bug can happen here
            // Mapping object from custom field to ViewModel manually
            var item = new FacilityItem
            {
                ItemName = { CustomField = availableCustomField.First(x => x.Name == "Item") },
                Reason =
                {
                    CustomField = availableCustomField.First(x => x.Name == "Reason for Repair/Improvement Request")
                }
            };

            viewModel.Items.Add(item);

            viewModel.Ticket.Owner = CurrentSession.UserId.ToString();

            ////await SetShopInfoForModelAsync(viewModel.Ticket).ConfigureAwait(false);

            return View("NewFacilityTicket", viewModel);
        }

        [NapAuth(FunctionKey = FunctionKeys.NewTicket, Policy = Constant.Policy.Read)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult NewFacilityTicket(NewFacilityTicketViewModel viewModel)
        {
            viewModel.Ticket.Shop = context.Shops.Find(1);
            viewModel.Ticket.CategoryId = (int)TicketCategoryTypes.Facility;
            ReValidateModelState(viewModel);

            if (ModelState.IsValid)
            {
                ticketService.CreateTicket(viewModel.Ticket, viewModel.TempId);

                customFieldService.AddCustomFieldValues(
                    viewModel.GetCustomFieldValuesToInsert(viewModel.Ticket.TicketId));

                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }

            ////if (ModelState.IsValid && await CreateTicketAsync(viewModel.Ticket, viewModel.TempId).ConfigureAwait(false))
            ////{
            ////    await AddTicketCustomField(viewModel).ConfigureAwait(false);

            ////    return new HttpStatusCodeResult(HttpStatusCode.OK);
            ////}

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        [NapAuth(FunctionKey = FunctionKeys.NewTicket, Policy = Constant.Policy.Read)]
        public ActionResult NewFeedbackTicket()
        {
            var availableCustomField = customFieldService.GetCustomFieldsForTicketCategory(TicketCategoryTypes.Feedback);

            var viewModel = new NewFeedbackTicketViewModel
            {
                TempId = Guid.NewGuid(),
                FeedbackBy = { CustomField = availableCustomField.First(x => x.Name == "Feedback by") },
                CustomerName = { CustomField = availableCustomField.First(x => x.Name == "Customer Name") },
                Feedback = { CustomField = availableCustomField.First(x => x.Name == "Feedback") },
                Ticket =
                {
                    Owner = CurrentSession.UserId.ToString()
                }
            };

            ////await SetShopInfoForModelAsync(viewModel.Ticket).ConfigureAwait(false);

            return View("NewFeedbackTicket", viewModel);
        }

        [NapAuth(FunctionKey = FunctionKeys.NewTicket, Policy = Constant.Policy.Read)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult NewFeedbackTicket(NewFeedbackTicketViewModel viewModel)
        {
            viewModel.Ticket.Shop = context.Shops.Find(1);
            viewModel.Ticket.CategoryId = (int)TicketCategoryTypes.Feedback;

            ReValidateModelState(viewModel);

            if (ModelState.IsValid)
            {
                ticketService.CreateTicket(viewModel.Ticket, viewModel.TempId);
                customFieldService.AddCustomFieldValues(
                    viewModel.GetCustomFieldValuesToInsert(viewModel.Ticket.TicketId));

                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }

            ////if (ModelState.IsValid && await CreateTicketAsync(viewModel.Ticket, viewModel.TempId).ConfigureAwait(false))
            ////{
            ////    customFieldService.AddCustomFieldValue(null, viewModel.FeedbackBy.CustomFieldId, viewModel.FeedbackBy.Value,
            ////        viewModel.Ticket.TicketId);
            ////    customFieldService.AddCustomFieldValue(null, viewModel.CustomerName.CustomFieldId, viewModel.CustomerName.Value,
            ////        viewModel.Ticket.TicketId);
            ////    customFieldService.AddCustomFieldValue(null, viewModel.Feedback.CustomFieldId, viewModel.Feedback.Value,
            ////        viewModel.Ticket.TicketId);

            ////    // Save changes to ticket's custom field
            ////    await Context.SaveChangesAsync().ConfigureAwait(false);

            ////    return new HttpStatusCodeResult(HttpStatusCode.OK);
            ////}

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        [NapAuth(FunctionKey = FunctionKeys.NewTicket, Policy = Constant.Policy.Read)]
        public ActionResult NewIncidentTicket()
        {
            var availableCustomField = customFieldService.GetCustomFieldsForTicketCategory(TicketCategoryTypes.Incident);

            var viewModel = new NewIncidentTicketViewModel
            {
                TempId = Guid.NewGuid(),
                TimeOfIncident = { CustomField = availableCustomField.First(x => x.Name == "Incident Date/Time") },
                Description = { CustomField = availableCustomField.First(x => x.Name == "Description") }
            };

            if (viewModel.TimeOfIncident.Value == null)
            {
                var nowDate = DateTime.Now.ToString("yyyy-MM-ddThh:mm");
                viewModel.TimeOfIncident.Value = nowDate;
            }
            viewModel.Ticket.ShopList = GetShopSelectList();

            ////await SetShopInfoForModelAsync(viewModel.Ticket).ConfigureAwait(false);

            return View("NewIncidentTicket", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateOnlyIncomingValues]
        [ValidateInput(false)]
        [NapAuth(FunctionKey = FunctionKeys.NewTicket, Policy = Constant.Policy.Read)]
        public ActionResult NewIncidentTicket(NewIncidentTicketViewModel viewModel)
        {
            if (viewModel.Ticket.IsHtml)
            {
                viewModel.Ticket.Details = viewModel.Ticket.Details.StripHtmlWhenEmpty();
                if (string.IsNullOrEmpty(viewModel.Ticket.Details))
                {
                    ModelState.AddModelError("Details", Strings.RequiredField);
                }
            }

            viewModel.Ticket.Shop = context.Shops.Find(1);
            viewModel.Ticket.CategoryId = (int)TicketCategoryTypes.Incident;

            if (ModelState.IsValid)
            {
                ticketService.CreateTicket(viewModel.Ticket, viewModel.TempId);
                customFieldService.AddCustomFieldValues(
                    viewModel.GetCustomFieldValuesToInsert(viewModel.Ticket.TicketId));

                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }

            ////if (ModelState.IsValid && await CreateTicketAsync(viewModel.Ticket, viewModel.TempId).ConfigureAwait(false))
            ////{
            ////    customFieldService.AddCustomFieldValue(null, viewModel.TimeOfIncident.CustomFieldId, viewModel.TimeOfIncident.Value, viewModel.Ticket.TicketId);
            ////    customFieldService.AddCustomFieldValue(null, viewModel.Description.CustomFieldId, viewModel.Description.Value, viewModel.Ticket.TicketId);

            ////    await Context.SaveChangesAsync().ConfigureAwait(false);

            ////    return new HttpStatusCodeResult(HttpStatusCode.OK);
            ////}

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        [NapAuth(FunctionKey = FunctionKeys.NewTicket, Policy = Constant.Policy.Read)]
        public ActionResult NewHardwareTicket()
        {
            var availableCustomField = customFieldService.GetCustomFieldsForTicketCategory(TicketCategoryTypes.Hardware);
            var viewModel = new NewHardwareTicketViewModel
            {
                TempId = Guid.NewGuid(),
                Ticket = {
                     ShopList = GetShopSelectList()
                },
                System = { CustomField = availableCustomField.First(x => x.Name == "Device") },
                TimeOfIncident = { CustomField = availableCustomField.First(x => x.Name == "Incident Date/Time") },
                Description = { CustomField = availableCustomField.First(x => x.Name == "Description") }
            };

            ////await SetShopInfoForModelAsync(viewModel.Ticket).ConfigureAwait(false);

            return View("NewHardwareTicket", viewModel);
        }

        [NapAuth(FunctionKey = FunctionKeys.NewTicket, Policy = Constant.Policy.Read)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateOnlyIncomingValues]
        [ValidateInput(false)]
        public ActionResult NewHardwareTicket(NewHardwareTicketViewModel viewModel)
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
            viewModel.Ticket.Shop = context.Shops.Find(1);
            viewModel.Ticket.CategoryId = (int)TicketCategoryTypes.Hardware;

            if (ModelState.IsValid)
            {
                ticketService.CreateTicket(viewModel.Ticket, viewModel.TempId);
                customFieldService.AddCustomFieldValues(
                    viewModel.GetCustomFieldValuesToInsert(viewModel.Ticket.TicketId));

                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }

            ////if (ModelState.IsValid && await CreateTicketAsync(viewModel.Ticket, viewModel.TempId).ConfigureAwait(false))
            ////{
            ////    customFieldService.AddCustomFieldValue(null, viewModel.System.CustomFieldId, viewModel.System.Value, viewModel.Ticket.TicketId);
            ////    customFieldService.AddCustomFieldValue(null, viewModel.TimeOfIncident.CustomFieldId, viewModel.TimeOfIncident.Value, viewModel.Ticket.TicketId);
            ////    customFieldService.AddCustomFieldValue(null, viewModel.Description.CustomFieldId, viewModel.Description.Value, viewModel.Ticket.TicketId);

            ////    // Save changes to ticket's custom field
            ////    await Context.SaveChangesAsync().ConfigureAwait(false);

            ////    return new HttpStatusCodeResult(HttpStatusCode.OK);
            ////}

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        [NapAuth(FunctionKey = FunctionKeys.NewTicket, Policy = Constant.Policy.Read)]
        public ActionResult NewSoftwareTicket()
        {
            var availableCustomField = customFieldService.GetCustomFieldsForTicketCategory(TicketCategoryTypes.Software);
            var viewModel = new NewSoftwareTicketViewModel
            {
                TempId = Guid.NewGuid(),
                Ticket = {
                    Owner = CurrentSession.UserId.ToString(),
                    ShopList = GetShopSelectList()
                },
                System = { CustomField = availableCustomField.First(x => x.Name == "System") },
                TimeOfIncident = { CustomField = availableCustomField.First(x => x.Name == "Incident Date/Time") },
                Description = { CustomField = availableCustomField.First(x => x.Name == "Description") }
            };

            ////await SetShopInfoForModelAsync(viewModel.Ticket).ConfigureAwait(false);

            return View("NewSoftwareTicket", viewModel);
        }

        [NapAuth(FunctionKey = FunctionKeys.NewTicket, Policy = Constant.Policy.Read)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateOnlyIncomingValues]
        [ValidateInput(false)]
        public ActionResult NewSoftwareTicket(NewSoftwareTicketViewModel viewModel)
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
            viewModel.Ticket.Shop = context.Shops.Find(1);
            viewModel.Ticket.CategoryId = (int)TicketCategoryTypes.Software;

            if (ModelState.IsValid)
            {
                ticketService.CreateTicket(viewModel.Ticket, viewModel.TempId);
                customFieldService.AddCustomFieldValues(
                    viewModel.GetCustomFieldValuesToInsert(viewModel.Ticket.TicketId));

                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }

            ////if (ModelState.IsValid && await CreateTicketAsync(viewModel.Ticket, viewModel.TempId).ConfigureAwait(false))
            ////{
            ////    customFieldService.AddCustomFieldValue(null, viewModel.System.CustomFieldId, viewModel.System.Value, viewModel.Ticket.TicketId);
            ////    customFieldService.AddCustomFieldValue(null, viewModel.TimeOfIncident.CustomFieldId, viewModel.TimeOfIncident.Value, viewModel.Ticket.TicketId);
            ////    customFieldService.AddCustomFieldValue(null, viewModel.Description.CustomFieldId, viewModel.Description.Value, viewModel.Ticket.TicketId);

            ////    // Save changes to ticket's custom field
            ////    await Context.SaveChangesAsync().ConfigureAwait(false);

            ////    return new HttpStatusCodeResult(HttpStatusCode.OK);
            ////}

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        public ActionResult NewCustomerSupportTicket()
        {
            var availableCustomField = customFieldService.GetCustomFieldsForTicketCategory(TicketCategoryTypes.CustomerSupport);

            var viewModel = new NewCustomerSupportTicketViewModel
            {
                TempId = Guid.NewGuid(),
                Ticket = {
                    Owner = CurrentSession.UserId.ToString(),
                    ShopList = GetShopSelectList()
                },
                SupportType = { CustomField = availableCustomField.First(x => x.Name == "Support Type") },
                TimeOfIncident = { CustomField = availableCustomField.First(x => x.Name == "Incident Date/Time") },
                NationalIdCardNo = { CustomField = availableCustomField.First(x => x.Name == "National Id Card No") },
                Description = { CustomField = availableCustomField.First(x => x.Name == "Description") }
            };

            ////await SetShopInfoForModelAsync(viewModel.Ticket).ConfigureAwait(false);

            return View("NewCustomerSupportTicket", viewModel);
        }

        [NapAuth(FunctionKey = FunctionKeys.NewTicket, Policy = Constant.Policy.Read)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateOnlyIncomingValues]
        public ActionResult NewCustomerSupportTicket(NewCustomerSupportTicketViewModel viewModel)
        {
            if (viewModel.Ticket.IsHtml)
            {
                viewModel.Ticket.Details = viewModel.Ticket.Details.StripHtmlWhenEmpty();
                if (string.IsNullOrEmpty(viewModel.Ticket.Details))
                {
                    ModelState.AddModelError("Details", Strings.RequiredField);
                }
            }

            viewModel.Ticket.Shop = context.Shops.Find(1);
            viewModel.Ticket.CategoryId = (int)TicketCategoryTypes.CustomerSupport;

            if (ModelState.IsValid)
            {
                ticketService.CreateTicket(viewModel.Ticket, viewModel.TempId);
                customFieldService.AddCustomFieldValues(
                    viewModel.GetCustomFieldValuesToInsert(viewModel.Ticket.TicketId));

                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }

            ////if (ModelState.IsValid && await CreateTicketAsync(viewModel.Ticket, viewModel.TempId).ConfigureAwait(false))
            ////{
            ////    customFieldService.AddCustomFieldValue(null, viewModel.NationalIdCardNo.CustomFieldId, viewModel.NationalIdCardNo.Value, viewModel.Ticket.TicketId);
            ////    customFieldService.AddCustomFieldValue(null, viewModel.TimeOfIncident.CustomFieldId, viewModel.TimeOfIncident.Value, viewModel.Ticket.TicketId);
            ////    customFieldService.AddCustomFieldValue(null, viewModel.Description.CustomFieldId, viewModel.Description.Value, viewModel.Ticket.TicketId);

            ////    await Context.SaveChangesAsync().ConfigureAwait(false);

            ////    return new HttpStatusCodeResult(HttpStatusCode.OK);
            ////}

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        [NapAuth(FunctionKey = FunctionKeys.TicketCenter, Policy = Constant.Policy.Read)]
        public ActionResult TicketDetails(int ticketId, int? categoryId)
        {
            var ticket = ticketService.GetTicket(ticketId);

            var singleTicketViewModel = new SingleTicketViewModel
            {
                Ticket = ticket,
                CustomFieldValues = customFieldService.GetCustomFieldValuesForTicket(ticket)
            };

            return PartialView("_TicketDetails", singleTicketViewModel);
        }

        [NapAuth(FunctionKey = FunctionKeys.TicketCenter, Policy = Constant.Policy.Read)]
        public ActionResult TicketEvents(int ticketId)
        {
            // WARNING! This is also used as a child action and cannot be made async in MVC 5
            var ticket = ticketService.GetTicket(ticketId);
            return PartialView("_TicketEvents", ticket?.TicketEvents);
        }

        [NapAuth(FunctionKey = FunctionKeys.TicketCenter, Policy = Constant.Policy.Read)]
        public ActionResult TicketFeedbackDetails(Ticket ticket)
        {
            // TODO: Set project id as shop id
            ////ticket.Shop = Context.Shops.First(x => x.ShopId == 1);

            var customFieldValues = customFieldService.GetCustomFieldValuesForTicket(ticket);

            var viewModel = new NewFeedbackTicketViewModel
            {
                FeedbackBy = customFieldValues.First(x => x.CustomField.Name == "Feedback by"),
                CustomerName = customFieldValues.First(x => x.CustomField.Name == "Customer Name"),
                Feedback = customFieldValues.First(x => x.CustomField.Name == "Feedback")
            };

            return PartialView("_FeedbackTicketDetails", viewModel);
        }

        [NapAuth(FunctionKey = FunctionKeys.TicketCenter, Policy = Constant.Policy.Read)]
        public ActionResult TicketHardwareDetails(Ticket ticket)
        {
            ////ticket.Shop = Context.Shops.First(x => x.ShopId == 1);
            var customFieldValues = customFieldService.GetCustomFieldValuesForTicket(ticket);
            var viewModel = new NewHardwareTicketViewModel
            {
                System = customFieldValues.First(x => x.CustomField.Name == "System"),
                Description = customFieldValues.First(x => x.CustomField.Name == "Description")
            };

            return PartialView("_HardwareTicketDetails", viewModel);
        }

        [NapAuth(FunctionKey = FunctionKeys.TicketCenter, Policy = Constant.Policy.Read)]
        public ActionResult TicketFiles(int ticketId)
        {
            // WARNING! This is also used as a child action and cannot be made async in MVC 5
            var attachments = TicketDeskFileStore.ListAttachmentInfo(ticketId.ToString(CultureInfo.InvariantCulture), false);
            ViewBag.TicketId = ticketId;
            return PartialView("_TicketFiles", attachments);
        }

        ////private void AddCustomFieldValue(string groupId, int customFieldId, string value, int ticketId)
        ////{
        ////    Context.CustomFieldValues.Add(new CustomFieldValue
        ////    {
        ////        GroupId = groupId,
        ////        CustomFieldId = customFieldId,
        ////        Value = value,
        ////        TicketId = ticketId
        ////    });
        ////}

        ////private async Task AddTicketCustomField(NewFacilityTicketViewModel viewModel)
        ////{
        ////    customFieldService.AddCustomFieldValue(null, viewModel.RequestType.CustomFieldId, viewModel.RequestType.Value, viewModel.Ticket.TicketId);

        ////    // add item to database
        ////    foreach (FacilityItem item in viewModel.Items)
        ////    {
        ////        customFieldService.AddCustomFieldValue(item.Index, item.ItemName.CustomFieldId, item.ItemName.Value, viewModel.Ticket.TicketId);
        ////        customFieldService.AddCustomFieldValue(item.Index, item.Reason.CustomFieldId, item.Reason.Value, viewModel.Ticket.TicketId);
        ////    }

        ////    // Save changes to ticket's custom field
        ////    await Context.SaveChangesAsync().ConfigureAwait(false);
        ////}

        ////private async Task<bool> CreateTicketAsync(Ticket ticket, Guid tempId)
        ////{
        ////    Context.Tickets.Add(ticket);
        ////    try
        ////    {
        ////        await Context.SaveChangesAsync().ConfigureAwait(false);
        ////        ticket.CommitPendingAttachments(tempId);
        ////    }
        ////    catch (DbEntityValidationException ex)
        ////    {
        ////        foreach (var entityValidationErrors in ex.EntityValidationErrors)
        ////        {
        ////            foreach (var validationError in entityValidationErrors.ValidationErrors)
        ////            {
        ////                Response.Write("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);
        ////            }
        ////        }
        ////    }

        ////    return ticket.TicketId != default(int);
        ////}

        ////private async Task SetShopInfoForModelAsync(Ticket ticket)
        ////{
        ////    if (ticket.ShopId == default(int))
        ////    {
        ////        var shops = await Context.Shops.Select(s => s.ShopId).ToListAsync().ConfigureAwait(false);
        ////        var isMulti = shops.Count > 1;
        ////        ViewBag.IsMultiProject = isMulti;

        ////        // set to first project if only one project exists, otherwise use user's selected project
        ////        ticket.ShopId = isMulti
        ////            ? await Context.UserSettingsManager.GetUserSelectedProjectIdAsync(Context).ConfigureAwait(false)
        ////            : shops.FirstOrDefault();
        ////    }
        ////}

        private IEnumerable<SelectListItem> GetShopSelectList()
        {
            var shops = context.Shops.Where(s => s.IsActive).OrderBy(s => s.ShopName).ToList();
            var selectListItem = shops.Select(s => new SelectListItem
            {
                Text = s.ShopName,
                Value = s.ShopId.ToString()
            });
            return selectListItem;
        }
    }
}