namespace TicketDesk.Web.Client.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using Domain;
    using Domain.Infrastructure;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;
    using System.Web.Mvc;
    using Domain.Model;
    using Models;
    using Models.CustomField;
    using System.Data.Entity;

    [RoutePrefix("admin/custom-field-settings")]
    [Route("{action=index}")]
    public class CustomFieldSettingsController : Controller
    {
        private readonly TdDomainContext _domainContext;

        public CustomFieldSettingsController(TdDomainContext domainContext)
        {
            _domainContext = domainContext;
        }

        [NapAuth(FunctionKey = FunctionKeys.CustomFieldSettings, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            var viewModel = new CustomFieldSettingViewModel
            {
                CustomFields = _domainContext.CustomFields.Include(x => x.TicketCategories)
            };

            return View(viewModel);
        }

        [Route("add")]
        [NapAuth(FunctionKey = FunctionKeys.CustomFieldSettings, Policy = Constant.Policy.Read)]
        public ActionResult Add()
        {
            AddCustomFieldViewModel viewModel = new AddCustomFieldViewModel
            {
                TicketCategories = new List<SelectListItem>(_domainContext.TicketCategories.Select(x =>
                    new SelectListItem
                    {
                        Text = x.Name,
                        Value = x.CategoryId.ToString()
                    })),
                CustomFieldTypes = new List<SelectListItem>(_domainContext.CustomFieldTypes.Select(x =>
                    new SelectListItem
                    {
                        Text = x.Name,
                        Value = x.Id.ToString()
                    }))
            };

            return View(viewModel);
        }

        [Route("add")]
        [HttpPost]
        [NapAuth(FunctionKey = FunctionKeys.CustomFieldSettings, Policy = Constant.Policy.Host)]
        public ActionResult Add(AddCustomFieldViewModel viewModel)
        {
            CustomField customField = new CustomField
            {
                Name = viewModel.Name,
                TicketCategories = new List<TicketCategory> { new TicketCategory(viewModel.TicketCategoryId) },
                CustomFieldType = new CustomFieldType { Id = viewModel.CustomFieldTypeId },
                Values = viewModel.Values
            };

            _domainContext.CustomFields.Add(customField);
            _domainContext.SaveChanges();

            return RedirectToAction("Add");
        }
    }
}