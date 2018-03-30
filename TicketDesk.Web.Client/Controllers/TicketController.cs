namespace TicketDesk.Web.Client.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Mvc;
    using Core.Enumerations;
    using Domain.Infrastructure;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;
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

        #region Constant for mapping from custom field to view models

        // Facility
        private const string REQUEST_TYPE = "Request Type";
        private const string ITEM = "Item";
        private const string REASON = "Reason for Repair/Improvement Request";

        // Feedback
        private const string FEEDBACK_BY = "Feedback by";
        private const string CUSTOMER_NAME = "Customer Name";
        private const string FEEDBACK = "Feedback";

        // Incident Report - Hardware - Customer Support
        private const string TIME_OF_INCIDENT = "Incident Date/Time";
        private const string DESCRIPTION = "Description";

        // Hardware
        private const string DEVICE = "Device";

        // Customer Support
        private const string SUPPORT_TYPE = "Support Type";
        private const string NATIONAL_ID_CARD_NO = "National Id Card No";

        // Software
        private const string SYSTEM = "System";

        #endregion

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

        [NapAuth(FunctionKey = FunctionKeys.Ticket, Policy = Constant.Policy.Host)]
        public ActionResult Index()
        {
            ViewBag.Title = Resources.Views_Ticket_Index_NewTicket;
            var items = GetIndexSubMenuItems();

            return View("_SubmenuNavigation", items);
        }

        [NapAuth(FunctionKey = FunctionKeys.Facility, Policy = Constant.Policy.Host)]
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
                RequestType = { CustomField = availableCustomField.First(x => x.Name == REQUEST_TYPE) },
                AllowToChooseRecipient = ticketCategoryService.GetTicketCategory((int)TicketCategoryTypes.Facility).IsAllowAdditionalMail
            };

            // Mapping object from custom field to ViewModel manually
            var item = new FacilityItem
            {
                ItemName = { CustomField = availableCustomField.First(x => x.Name == ITEM) },
                Reason =
                {
                    CustomField = availableCustomField.First(x => x.Name == REASON)
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
                    Resources.NewTicketInValidEmailErrorMessage);
            }

            if (ModelState.IsValid)
            {
                result = ticketService.CreateTicket(viewModel.Ticket, viewModel.TempId);

                if (result)
                {
                    result = customFieldService.AddCustomFieldValues(
                        viewModel.GetCustomFieldValuesToInsert(viewModel.Ticket.TicketId));

                    mailService.ComposeAndSendMail(viewModel.Ticket);
                }
            }

            return Json(new
            {
                result,
                errors = ModelState
                    .Where(item => item.Value.Errors.Any())
                    .Select(item => new { item.Key, item.Value.Errors.FirstOrDefault()?.ErrorMessage })
            });
        }

        [NapAuth(FunctionKey = FunctionKeys.Feedback, Policy = Constant.Policy.Host)]
        public ActionResult NewFeedbackTicket()
        {
            var availableCustomField = customFieldService.GetCustomFieldsForTicketCategory(TicketCategoryTypes.Feedback);

            var viewModel = new NewFeedbackTicketViewModel
            {
                TempId = Guid.NewGuid(),
                FeedbackBy = { CustomField = availableCustomField.First(x => x.Name == FEEDBACK_BY) },
                CustomerName = { CustomField = availableCustomField.First(x => x.Name == CUSTOMER_NAME) },
                Feedback = { CustomField = availableCustomField.First(x => x.Name == FEEDBACK) },
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
                    Resources.NewTicketInValidEmailErrorMessage);
            }

            if (ModelState.IsValid)
            {
                result = ticketService.CreateTicket(viewModel.Ticket, viewModel.TempId);
                if (result)
                {
                    result = customFieldService.AddCustomFieldValues(
                        viewModel.GetCustomFieldValuesToInsert(viewModel.Ticket.TicketId));

                    mailService.ComposeAndSendMail(viewModel.Ticket);
                }
            }

            return Json(new
            {
                result,
                errors = ModelState
                    .Where(item => item.Value.Errors.Any())
                    .Select(item => new { item.Key, item.Value.Errors.FirstOrDefault()?.ErrorMessage })
            });
        }

        [NapAuth(FunctionKey = FunctionKeys.Incident, Policy = Constant.Policy.Host)]
        public ActionResult NewIncidentTicket()
        {
            var availableCustomField = customFieldService.GetCustomFieldsForTicketCategory(TicketCategoryTypes.Incident);

            var viewModel = new NewIncidentTicketViewModel
            {
                TempId = Guid.NewGuid(),
                TimeOfIncident = { CustomField = availableCustomField.First(x => x.Name == TIME_OF_INCIDENT) },
                Description = { CustomField = availableCustomField.First(x => x.Name == DESCRIPTION) },
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

            if (!mailService.IsEmailListValid(viewModel.Ticket.AdditionalMail))
            {
                ModelState.AddModelError(
                    $"{nameof(viewModel.Ticket)}.{nameof(viewModel.Ticket.AdditionalMail)}",
                    Resources.NewTicketInValidEmailErrorMessage);
            }

            if (ModelState.IsValid)
            {
                result = ticketService.CreateTicket(viewModel.Ticket, viewModel.TempId);
                if (result)
                {
                    result = customFieldService.AddCustomFieldValues(
                        viewModel.GetCustomFieldValuesToInsert(viewModel.Ticket.TicketId));

                    mailService.ComposeAndSendMail(viewModel.Ticket);
                }
            }

            return Json(new
            {
                result,
                errors = ModelState
                    .Where(item => item.Value.Errors.Any())
                    .Select(item => new { item.Key, item.Value.Errors.FirstOrDefault()?.ErrorMessage })
            });
        }

        [NapAuth(FunctionKey = FunctionKeys.Hardware, Policy = Constant.Policy.Host)]
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
                System = { CustomField = availableCustomField.First(x => x.Name == DEVICE) },
                TimeOfIncident = { CustomField = availableCustomField.First(x => x.Name == TIME_OF_INCIDENT) },
                Description = { CustomField = availableCustomField.First(x => x.Name == DESCRIPTION) },
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
                    Resources.NewTicketInValidEmailErrorMessage);
            }

            if (ModelState.IsValid)
            {
                result = ticketService.CreateTicket(viewModel.Ticket, viewModel.TempId);

                if (result)
                {
                    result = customFieldService.AddCustomFieldValues(
                        viewModel.GetCustomFieldValuesToInsert(viewModel.Ticket.TicketId));

                    mailService.ComposeAndSendMail(viewModel.Ticket);
                }
            }

            return Json(new
            {
                result,
                errors = ModelState
                    .Where(item => item.Value.Errors.Any())
                    .Select(item => new { item.Key, item.Value.Errors.FirstOrDefault()?.ErrorMessage })
            });
        }

        [NapAuth(FunctionKey = FunctionKeys.Software, Policy = Constant.Policy.Host)]
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
                System = { CustomField = availableCustomField.First(x => x.Name == SYSTEM) },
                TimeOfIncident = { CustomField = availableCustomField.First(x => x.Name == TIME_OF_INCIDENT) },
                Description = { CustomField = availableCustomField.First(x => x.Name == DESCRIPTION) },
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
                    Resources.NewTicketInValidEmailErrorMessage);
            }

            if (ModelState.IsValid)
            {
                result = ticketService.CreateTicket(viewModel.Ticket, viewModel.TempId);
                if (result)
                {
                    result = customFieldService.AddCustomFieldValues(
                        viewModel.GetCustomFieldValuesToInsert(viewModel.Ticket.TicketId));

                    mailService.ComposeAndSendMail(viewModel.Ticket);
                }
            }

            return Json(new
            {
                result,
                errors = ModelState
                    .Where(item => item.Value.Errors.Any())
                    .Select(item => new { item.Key, item.Value.Errors.FirstOrDefault()?.ErrorMessage })
            });
        }

        [NapAuth(FunctionKey = FunctionKeys.CustomerSupport, Policy = Constant.Policy.Host)]
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
                SupportType = { CustomField = availableCustomField.First(x => x.Name == SUPPORT_TYPE) },
                TimeOfIncident = { CustomField = availableCustomField.First(x => x.Name == TIME_OF_INCIDENT) },
                NationalIdCardNo = { CustomField = availableCustomField.First(x => x.Name == NATIONAL_ID_CARD_NO) },
                Description = { CustomField = availableCustomField.First(x => x.Name == DESCRIPTION) },
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
                    Resources.NewTicketInValidEmailErrorMessage);
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

                    mailService.ComposeAndSendMail(viewModel.Ticket);
                }
            }

            return Json(new
            {
                result,
                errors = ModelState
                    .Where(item => item.Value.Errors.Any())
                    .Select(item => new { item.Key, item.Value.Errors.FirstOrDefault()?.ErrorMessage })
            });
        }
    }
}