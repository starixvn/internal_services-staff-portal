namespace TicketDesk.Web.Client.Controllers
{
    using Domain;
    using Domain.Infrastructure;
    using Domain.Model;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;
    using Models;
    using Models.CustomField;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

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
        public ActionResult Index(int id = 1)
        {
            var categoryIds = _domainContext.TicketCategoryCustomField.Select(x => x.CategoryId);

            var viewModel = new CustomFieldSettingViewModel
            {
                TicketCategoryWithCustomFields =
                    _domainContext.TicketCategories.Where(x => categoryIds.Contains(x.CategoryId))
            };

            var customFieldIds = _domainContext.TicketCategoryCustomField.Where(x => x.CategoryId == id)
                .Select(x => x.CustomFieldId);

            viewModel.CustomFields = _domainContext.CustomFields.Where(x => customFieldIds.Contains(x.Id));

            foreach (CustomField customField in viewModel.CustomFields)
            {
                customField.CustomFieldType =
                    _domainContext.CustomFieldTypes.First(x => x.Id == customField.CustomFieldTypeId);
            }

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
                CustomFieldTypeId = viewModel.CustomFieldTypeId,
                Values = viewModel.Values
            };

            _domainContext.CustomFields.Add(customField);

            _domainContext.SaveChanges();

            foreach (var id in viewModel.TicketCategoryIds)
            {
                var ticketCategoryCustomField = new TicketCategoryCustomField
                {
                    CategoryId = id,
                    CustomFieldId = customField.Id
                };

                _domainContext.TicketCategoryCustomField.Add(ticketCategoryCustomField);
            }

            _domainContext.SaveChanges();

            return RedirectToAction("Index");
        }

        [Route("edit")]
        [NapAuth(FunctionKey = FunctionKeys.CustomFieldSettings, Policy = Constant.Policy.Read)]
        public ActionResult Edit(int id)
        {
            CustomField customField = _domainContext.CustomFields.First(x => x.Id == id);

            var categoriesHaveCustomField = _domainContext.TicketCategoryCustomField.Where(x => x.CustomFieldId == id)
                .Select(x => x.CategoryId);

            AddCustomFieldViewModel viewModel = new AddCustomFieldViewModel
            {
                TicketCategories = new List<SelectListItem>(_domainContext.TicketCategories.Select(x =>
                    new SelectListItem
                    {
                        Text = x.Name,
                        Value = x.CategoryId.ToString(),
                        Selected = categoriesHaveCustomField.Contains(x.CategoryId)
                    })),
                Name = customField.Name,
                Values = customField.Values,
                DisplayOrder = customField.DisplayOrder,
                IsEnabled = customField.IsEnabled,
                CustomFieldTypeId = customField.CustomFieldTypeId
            };

            return View(viewModel);
        }

        [Route("edit")]
        [HttpPost]
        [NapAuth(FunctionKey = FunctionKeys.CustomFieldSettings, Policy = Constant.Policy.Host)]
        public ActionResult Edit(AddCustomFieldViewModel viewModel)
        {
            return RedirectToAction("Index");
        }

        [Route("delete")]
        [NapAuth(FunctionKey = FunctionKeys.CustomFieldSettings, Policy = Constant.Policy.Host)]
        public ActionResult Delete(int id)
        {
            CustomField customFieldToDelete = new CustomField
            {
                Id = id
            };

            _domainContext.CustomFields.Attach(customFieldToDelete);
            _domainContext.CustomFields.Remove(customFieldToDelete);

            _domainContext.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}