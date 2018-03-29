namespace TicketDesk.Web.Client.Controllers
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Web.Mvc;
    using Core.Enumerations;
    using Domain.Infrastructure;
    using Domain.Model;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;
    using IO;
    using Localization.Client;
    using Models.Ticket;
    using Service.Interface;

    public class TicketController : BaseController
    {
        private readonly ITicketService ticketService;
        private readonly ICustomFieldService customFieldService;
        private readonly IShopService shopService;
        private readonly ITicketCategoryService ticketCategoryService;
        private readonly IMailService mailService;

        public TicketController(
            ITicketService ticketService,
            ICustomFieldService customFieldService,
            IShopService shopService,
            ITicketCategoryService ticketCategoryService,
            IMailService mailService)
        {
            this.ticketService = ticketService;
            this.customFieldService = customFieldService;
            this.shopService = shopService;
            this.ticketCategoryService = ticketCategoryService;
            this.mailService = mailService;
        }

        [HttpPost]
        [NapAuth(FunctionKey = FunctionKeys.NewTicket, Policy = Constant.Policy.Read)]
        public JsonResult ChangeTicketSubscription(int ticketId)
        {
            var isSubscribed = ticketService.UpdateTicketSubscription(ticketId, CurrentSession.UserId.ToString());
            return new JsonCamelCaseResult { Data = new { IsSubscribed = isSubscribed } };
        }

        [NapAuth(FunctionKey = FunctionKeys.NewTicket, Policy = Constant.Policy.Read)]
        public ActionResult FacilityTicketDetails(Ticket ticket)
        {
            var viewModel = new NewFacilityTicketViewModel
            {
                Ticket = ticket
            };

            return PartialView("_FacilityTicketDetails", viewModel);
        }

        [NapAuth(FunctionKey = FunctionKeys.NewTicket, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            ViewBag.Title = Resources.Views_Ticket_Index_NewTicket;
            var items = GetIndexSubMenuItems();

            return View("_SubmenuNavigation", items);
        }

        [NapAuth(FunctionKey = FunctionKeys.Facility, Policy = Constant.Policy.Read)]
        public ActionResult NewFacilityTicket()
        {
            var availableCustomField = customFieldService.GetCustomFieldsForTicketCategory(TicketCategoryTypes.Facility);

            var viewModel = new NewFacilityTicketViewModel
            {
                TempId = Guid.NewGuid(),
                Ticket =
                {
                    ShopList = shopService.GetShopSelectList()
                },
                RequestType = { CustomField = availableCustomField.First(x => x.Name == "Request Type") },
                AllowToChooseRecipient = ticketCategoryService.GetTicketCategory((int)TicketCategoryTypes.Facility).IsAllowAdditionalMail
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

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateOnlyIncomingValues]
        [NapAuth(FunctionKey = FunctionKeys.Facility, Policy = Constant.Policy.Host)]
        public ActionResult NewFacilityTicket(NewFacilityTicketViewModel viewModel)
        {
            viewModel.Ticket.CategoryId = (int)TicketCategoryTypes.Facility;

            var result = false;
            if (!mailService.IsEmailListValid(viewModel.Ticket.AdditionalMail))
            {
                ModelState.AddModelError(
                    $"{nameof(viewModel.Ticket)}.{nameof(viewModel.Ticket.AdditionalMail)}",
                    Resources.ToEmailErrorMessage);
            }

            if (ModelState.IsValid)
            {
                result = ticketService.CreateTicket(viewModel.Ticket, viewModel.TempId);

                if (result)
                {
                    result = customFieldService.AddCustomFieldValues(
                        viewModel.GetCustomFieldValuesToInsert(viewModel.Ticket.TicketId));
                }

                mailService.ComposeAndSendMail(viewModel.Ticket);
            }

            return Json(new
            {
                result,
                errors = ModelState.Values.SelectMany(m => m.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList()
            });
        }

        [NapAuth(FunctionKey = FunctionKeys.Feedback, Policy = Constant.Policy.Read)]
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
                    Owner = CurrentSession.UserId.ToString(),
                    ShopList = shopService.GetShopSelectList()
                },
                AllowToChooseRecipient = ticketCategoryService.GetTicketCategory((int)TicketCategoryTypes.Feedback).IsAllowAdditionalMail
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateOnlyIncomingValues]
        [NapAuth(FunctionKey = FunctionKeys.Feedback, Policy = Constant.Policy.Host)]
        public ActionResult NewFeedbackTicket(NewFeedbackTicketViewModel viewModel)
        {
            var result = false;

            viewModel.Ticket.CategoryId = (int)TicketCategoryTypes.Feedback;

            if (!mailService.IsEmailListValid(viewModel.Ticket.AdditionalMail))
            {
                ModelState.AddModelError(
                    $"{nameof(viewModel.Ticket)}.{nameof(viewModel.Ticket.AdditionalMail)}",
                    Resources.ToEmailErrorMessage);
            }

            if (ModelState.IsValid)
            {
                result = ticketService.CreateTicket(viewModel.Ticket, viewModel.TempId);
                if (result)
                {
                    result = customFieldService.AddCustomFieldValues(
                        viewModel.GetCustomFieldValuesToInsert(viewModel.Ticket.TicketId));
                }

                mailService.ComposeAndSendMail(viewModel.Ticket);
            }

            return Json(new
            {
                result,
                errors = ModelState.Values.SelectMany(m => m.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList()
            });
        }

        [NapAuth(FunctionKey = FunctionKeys.Incident, Policy = Constant.Policy.Read)]
        public ActionResult NewIncidentTicket()
        {
            var availableCustomField = customFieldService.GetCustomFieldsForTicketCategory(TicketCategoryTypes.Incident);

            var viewModel = new NewIncidentTicketViewModel
            {
                TempId = Guid.NewGuid(),
                TimeOfIncident = { CustomField = availableCustomField.First(x => x.Name == "Incident Date/Time") },
                Description = { CustomField = availableCustomField.First(x => x.Name == "Description") },
                AllowToChooseRecipient = ticketCategoryService.GetTicketCategory((int)TicketCategoryTypes.Incident).IsAllowAdditionalMail
            };

            if (viewModel.TimeOfIncident.Value == null)
            {
                var nowDate = DateTime.Now.ToString("yyyy-MM-ddThh:mm");
                viewModel.TimeOfIncident.Value = nowDate;
            }

            viewModel.Ticket.ShopList = shopService.GetShopSelectList();

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateOnlyIncomingValues]
        [NapAuth(FunctionKey = FunctionKeys.Incident, Policy = Constant.Policy.Host)]
        public ActionResult NewIncidentTicket(NewIncidentTicketViewModel viewModel)
        {
            var result = false;

            viewModel.Ticket.CategoryId = (int)TicketCategoryTypes.Incident;

            if (ModelState.IsValid)
            {
                result = ticketService.CreateTicket(viewModel.Ticket, viewModel.TempId);
                if (result)
                {
                    result = customFieldService.AddCustomFieldValues(
                        viewModel.GetCustomFieldValuesToInsert(viewModel.Ticket.TicketId));
                }

                mailService.ComposeAndSendMail(viewModel.Ticket);
            }

            return Json(new
            {
                result,
                errors = ModelState.Values.SelectMany(m => m.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList()
            });
        }

        [NapAuth(FunctionKey = FunctionKeys.Hardware, Policy = Constant.Policy.Read)]
        public ActionResult NewHardwareTicket()
        {
            var availableCustomField = customFieldService.GetCustomFieldsForTicketCategory(TicketCategoryTypes.Hardware);
            var viewModel = new NewHardwareTicketViewModel
            {
                TempId = Guid.NewGuid(),
                Ticket =
                {
                     ShopList = shopService.GetShopSelectList()
                },
                System = { CustomField = availableCustomField.First(x => x.Name == "Device") },
                TimeOfIncident = { CustomField = availableCustomField.First(x => x.Name == "Incident Date/Time") },
                Description = { CustomField = availableCustomField.First(x => x.Name == "Description") },
                AllowToChooseRecipient = ticketCategoryService.GetTicketCategory((int)TicketCategoryTypes.Hardware).IsAllowAdditionalMail
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateOnlyIncomingValues]
        [NapAuth(FunctionKey = FunctionKeys.Hardware, Policy = Constant.Policy.Host)]
        public ActionResult NewHardwareTicket(NewHardwareTicketViewModel viewModel)
        {
            viewModel.Ticket.CategoryId = (int)TicketCategoryTypes.Hardware;

            var result = false;
            if (!mailService.IsEmailListValid(viewModel.Ticket.AdditionalMail))
            {
                ModelState.AddModelError(
                    $"{nameof(viewModel.Ticket)}.{nameof(viewModel.Ticket.AdditionalMail)}",
                    Resources.ToEmailErrorMessage);
            }

            if (ModelState.IsValid)
            {
                result = ticketService.CreateTicket(viewModel.Ticket, viewModel.TempId);

                if (result)
                {
                    result = customFieldService.AddCustomFieldValues(
                        viewModel.GetCustomFieldValuesToInsert(viewModel.Ticket.TicketId));
                }

                mailService.ComposeAndSendMail(viewModel.Ticket);
            }

            return Json(new
            {
                result,
                errors = ModelState.Values.SelectMany(m => m.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList()
            });
        }

        [NapAuth(FunctionKey = FunctionKeys.Software, Policy = Constant.Policy.Read)]
        public ActionResult NewSoftwareTicket()
        {
            var availableCustomField = customFieldService.GetCustomFieldsForTicketCategory(TicketCategoryTypes.Software);
            var viewModel = new NewSoftwareTicketViewModel
            {
                TempId = Guid.NewGuid(),
                Ticket = {
                    Owner = CurrentSession.UserId.ToString(),
                    ShopList = shopService.GetShopSelectList()
                },
                System = { CustomField = availableCustomField.First(x => x.Name == "System") },
                TimeOfIncident = { CustomField = availableCustomField.First(x => x.Name == "Incident Date/Time") },
                Description = { CustomField = availableCustomField.First(x => x.Name == "Description") },
                AllowToChooseRecipient = ticketCategoryService.GetTicketCategory((int)TicketCategoryTypes.Software).IsAllowAdditionalMail
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateOnlyIncomingValues]
        [NapAuth(FunctionKey = FunctionKeys.Software, Policy = Constant.Policy.Host)]
        public ActionResult NewSoftwareTicket(NewSoftwareTicketViewModel viewModel)
        {
            viewModel.Ticket.CategoryId = (int)TicketCategoryTypes.Software;

            var result = false;
            if (!mailService.IsEmailListValid(viewModel.Ticket.AdditionalMail))
            {
                ModelState.AddModelError(
                    $"{nameof(viewModel.Ticket)}.{nameof(viewModel.Ticket.AdditionalMail)}",
                    Resources.ToEmailErrorMessage);
            }

            if (ModelState.IsValid)
            {
                result = ticketService.CreateTicket(viewModel.Ticket, viewModel.TempId);
                if (result)
                {
                    result = customFieldService.AddCustomFieldValues(
                        viewModel.GetCustomFieldValuesToInsert(viewModel.Ticket.TicketId));
                }

                mailService.ComposeAndSendMail(viewModel.Ticket);
            }

            return Json(new
            {
                result,
                errors = ModelState.Values.SelectMany(m => m.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList()
            });
        }

        [NapAuth(FunctionKey = FunctionKeys.CustomerSupport, Policy = Constant.Policy.Read)]
        public ActionResult NewCustomerSupportTicket()
        {
            var availableCustomField = customFieldService.GetCustomFieldsForTicketCategory(TicketCategoryTypes.CustomerSupport);

            var viewModel = new NewCustomerSupportTicketViewModel
            {
                TempId = Guid.NewGuid(),
                Ticket =
                {
                    Owner = CurrentSession.UserId.ToString(),
                    ShopList = shopService.GetShopSelectList()
                },
                SupportType = { CustomField = availableCustomField.First(x => x.Name == "Support Type") },
                TimeOfIncident = { CustomField = availableCustomField.First(x => x.Name == "Incident Date/Time") },
                NationalIdCardNo = { CustomField = availableCustomField.First(x => x.Name == "National Id Card No") },
                Description = { CustomField = availableCustomField.First(x => x.Name == "Description") },
                AllowToChooseRecipient = ticketCategoryService.GetTicketCategory((int)TicketCategoryTypes.CustomerSupport).IsAllowAdditionalMail
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateOnlyIncomingValues]
        [NapAuth(FunctionKey = FunctionKeys.CustomerSupport, Policy = Constant.Policy.Host)]
        public ActionResult NewCustomerSupportTicket(NewCustomerSupportTicketViewModel viewModel)
        {
            viewModel.Ticket.CategoryId = (int)TicketCategoryTypes.CustomerSupport;

            var result = false;
            if (!mailService.IsEmailListValid(viewModel.Ticket.AdditionalMail))
            {
                ModelState.AddModelError(
                    $"{nameof(viewModel.Ticket)}.{nameof(viewModel.Ticket.AdditionalMail)}",
                    Resources.ToEmailErrorMessage);
            }

            if (ModelState.IsValid)
            {
                var ticketTitle = "[" + viewModel.SupportType.Value + "] - " + viewModel.Ticket.Title;
                viewModel.Ticket.Title = ticketTitle;
                result = ticketService.CreateTicket(viewModel.Ticket, viewModel.TempId);

                if (result)
                {
                    result = customFieldService.AddCustomFieldValues(
                        viewModel.GetCustomFieldValuesToInsert(viewModel.Ticket.TicketId));
                }

                mailService.ComposeAndSendMail(viewModel.Ticket);
            }

            return Json(new
            {
                result,
                errors = ModelState.Values.SelectMany(m => m.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList()
            });
        }

        [NapAuth(FunctionKey = FunctionKeys.NewTicket, Policy = Constant.Policy.Read)]
        public ActionResult TicketEvents(int ticketId)
        {
            // WARNING! This is also used as a child action and cannot be made async in MVC 5
            var ticket = ticketService.GetTicket(ticketId);
            return PartialView("_TicketEvents", ticket?.TicketEvents);
        }

        [NapAuth(FunctionKey = FunctionKeys.NewTicket, Policy = Constant.Policy.Read)]
        public ActionResult TicketFeedbackDetails(Ticket ticket)
        {
            var customFieldValues = customFieldService.GetCustomFieldValuesForTicket(ticket);

            var viewModel = new NewFeedbackTicketViewModel
            {
                FeedbackBy = customFieldValues.First(x => x.CustomField.Name == "Feedback by"),
                CustomerName = customFieldValues.First(x => x.CustomField.Name == "Customer Name"),
                Feedback = customFieldValues.First(x => x.CustomField.Name == "Feedback")
            };

            return PartialView("_FeedbackTicketDetails", viewModel);
        }

        [NapAuth(FunctionKey = FunctionKeys.NewTicket, Policy = Constant.Policy.Read)]
        public ActionResult TicketHardwareDetails(Ticket ticket)
        {
            var customFieldValues = customFieldService.GetCustomFieldValuesForTicket(ticket);
            var viewModel = new NewHardwareTicketViewModel
            {
                System = customFieldValues.First(x => x.CustomField.Name == "System"),
                Description = customFieldValues.First(x => x.CustomField.Name == "Description")
            };

            return PartialView("_HardwareTicketDetails", viewModel);
        }

        [NapAuth(FunctionKey = FunctionKeys.NewTicket, Policy = Constant.Policy.Read)]
        public ActionResult TicketFiles(int ticketId)
        {
            // WARNING! This is also used as a child action and cannot be made async in MVC 5
            var attachments = TicketDeskFileStore.ListAttachmentInfo(ticketId.ToString(CultureInfo.InvariantCulture), false);
            ViewBag.TicketId = ticketId;
            return PartialView("_TicketFiles", attachments);
        }
    }
}