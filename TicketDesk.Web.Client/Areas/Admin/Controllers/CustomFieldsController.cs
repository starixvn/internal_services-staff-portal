namespace TicketDesk.Web.Client.Areas.Admin.Controllers
{
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Web.Mvc;
    using Domain;
    using Domain.Infrastructure;
    using Domain.Model;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;
    using Models.CustomField;

    [TdAuthorize]
    public class CustomFieldsController : Controller
    {
        private readonly TdDomainContext _domainContext;

        public CustomFieldsController(TdDomainContext domainContext)
        {
            _domainContext = domainContext;
        }

        [NapAuth(FunctionKey = FunctionKeys.CustomFieldSettings, Policy = Constant.Policy.Read)]
        public ActionResult Index(int id = 1)
        {
            var categoryIds = _domainContext.TicketCategoryCustomField.Select(x => x.CategoryId);

            var viewModel = new CustomFieldSettingViewModel
            {
                SelectedCategoryId = id,
                TicketCategoryWithCustomFields =
                    _domainContext.TicketCategories.Where(x => categoryIds.Contains(x.CategoryId))
            };

            var customFieldIds = _domainContext.TicketCategoryCustomField.Where(x => x.CategoryId == id)
                .Select(x => x.CustomFieldId);

            viewModel.CustomFields = _domainContext.CustomFields.Where(x => customFieldIds.Contains(x.Id));

            foreach (var customField in viewModel.CustomFields)
            {
                customField.CustomFieldType =
                    _domainContext.CustomFieldTypes.First(x => x.Id == customField.CustomFieldTypeId);
            }

            return View(viewModel);
        }

        [NapAuth(FunctionKey = FunctionKeys.CustomFieldSettings, Policy = Constant.Policy.Read)]
        public ActionResult Add()
        {
            var viewModel = new AddCustomFieldViewModel
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

        [HttpPost]
        [NapAuth(FunctionKey = FunctionKeys.CustomFieldSettings, Policy = Constant.Policy.Host)]
        public ActionResult Add(AddCustomFieldViewModel viewModel)
        {
            var customField = new CustomField
            {
                Name = viewModel.Name,
                Name_vi = viewModel.Name_vi,
                CustomFieldTypeId = viewModel.CustomFieldTypeId,
                Values = viewModel.Values,
                Values_vi = viewModel.Values_vi
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

        [NapAuth(FunctionKey = FunctionKeys.CustomFieldSettings, Policy = Constant.Policy.Read)]
        public ActionResult Edit(int id)
        {
            var customField = _domainContext.CustomFields.First(x => x.Id == id);

            var categoriesHaveCustomField = _domainContext.TicketCategoryCustomField.Where(x => x.CustomFieldId == id)
                .Select(x => x.CategoryId);

            var viewModel = new EditCustomFieldViewModel
            {
                Id = id,
                TicketCategories = new List<SelectListItem>(_domainContext.TicketCategories.Select(x =>
                    new SelectListItem
                    {
                        Text = x.Name,
                        Value = x.CategoryId.ToString(),
                        Selected = categoriesHaveCustomField.Contains(x.CategoryId)
                    })),
                Name = customField.Name,
                Name_vi = customField.Name_vi,
                Values = customField.Values,
                Values_vi = customField.Values_vi,
                DisplayOrder = customField.DisplayOrder,
                IsEnabled = customField.IsEnabled,
                CustomFieldTypeId = customField.CustomFieldTypeId
            };

            return View(viewModel);
        }

        [HttpPost]
        [NapAuth(FunctionKey = FunctionKeys.CustomFieldSettings, Policy = Constant.Policy.Host)]
        public ActionResult Edit(EditCustomFieldViewModel viewModel)
        {
            var customField = _domainContext.CustomFields.First(x => x.Id == viewModel.Id);

            customField.Name = viewModel.Name;
            customField.Name_vi = viewModel.Name_vi;
            customField.Values_vi = viewModel.Values_vi;
            customField.Values = viewModel.Values;
            customField.DisplayOrder = viewModel.DisplayOrder;
            customField.IsEnabled = viewModel.IsEnabled;

            _domainContext.CustomFields.AddOrUpdate(customField);

            _domainContext.SaveChanges();

            // Update ticket categories
            var ticketCategoryCustomFields =
                _domainContext.TicketCategoryCustomField.Where(x => x.CustomFieldId == viewModel.Id);

            _domainContext.TicketCategoryCustomField.RemoveRange(ticketCategoryCustomFields);

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

        [NapAuth(FunctionKey = FunctionKeys.CustomFieldSettings, Policy = Constant.Policy.Host)]
        public ActionResult Delete(int id)
        {
            var customFieldToDelete = new CustomField
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