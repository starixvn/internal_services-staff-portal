namespace TicketDesk.Web.Client.Areas.Admin.Controllers
{
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Web.Mvc;
    using Client.Controllers;
    using Domain;
    using Domain.Infrastructure;
    using Domain.Model;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;
    using Models.CustomField;

    [NapAuth(FunctionKey = FunctionKeys.CustomFields, Policy = Constant.Policy.Read)]
    public class CustomFieldsController : BaseController
    {
        private readonly TdDomainContext _context;

        public CustomFieldsController(TdDomainContext context)
        {
            _context = context;
        }

        public ActionResult Index(int id = 1)
        {
            var categoryIds = _context.TicketCategoryCustomField.Select(x => x.CategoryId);

            var viewModel = new CustomFieldsViewModel
            {
                SelectedCategoryId = id,
                TicketCategoryWithCustomFields =
                    _context.TicketCategories.Where(x => categoryIds.Contains(x.CategoryId))
            };

            var customFieldIds = _context.TicketCategoryCustomField.Where(x => x.CategoryId == id)
                .Select(x => x.CustomFieldId);

            viewModel.CustomFields = _context.CustomFields.Where(x => customFieldIds.Contains(x.Id));

            foreach (var customField in viewModel.CustomFields)
            {
                customField.CustomFieldType = _context.CustomFieldTypes.Find(customField.CustomFieldTypeId);
            }

            return View(viewModel);
        }

        public ActionResult Add()
        {
            var viewModel = new AddCustomFieldViewModel
            {
                TicketCategories = new List<SelectListItem>(_context.TicketCategories.Select(x =>
                    new SelectListItem
                    {
                        Text = x.Name,
                        Value = x.CategoryId.ToString()
                    })),
                CustomFieldTypes = new List<SelectListItem>(_context.CustomFieldTypes.Select(x =>
                    new SelectListItem
                    {
                        Text = x.Name,
                        Value = x.Id.ToString()
                    }))
            };

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Add(AddCustomFieldViewModel viewModel)
        {
            var customField = new CustomField
            {
                Name = viewModel.Name,
                Name_vi = viewModel.Name_vi,
                CustomFieldTypeId = viewModel.CustomFieldTypeId,
                Values = viewModel.Values,
                Values_vi = viewModel.Values_vi,
                IsEnabled = viewModel.IsEnabled,
                IsAllowedToEditListValue = viewModel.IsAllowedToEditListValue
            };

            _context.CustomFields.Add(customField);

            _context.SaveChanges();

            foreach (var id in viewModel.TicketCategoryIds)
            {
                var ticketCategoryCustomField = new TicketCategoryCustomField
                {
                    CategoryId = id,
                    CustomFieldId = customField.Id
                };

                _context.TicketCategoryCustomField.Add(ticketCategoryCustomField);
            }

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            var customField = _context.CustomFields.Find(id);

            var categoriesHaveCustomField = _context.TicketCategoryCustomField.Where(x => x.CustomFieldId == id)
                .Select(x => x.CategoryId);

            if (customField != null)
            {
                var viewModel = new EditCustomFieldViewModel
                {
                    Id = id,
                    TicketCategories = new List<SelectListItem>(_context.TicketCategories.Select(x =>
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
                    IsAllowedToEditListValue = customField.IsAllowedToEditListValue,
                    CustomFieldTypeId = customField.CustomFieldTypeId
                };

                return View(viewModel);
            }

            return RedirectToAction("Index", "CustomFields");
        }

        [HttpPost]
        public ActionResult Edit(EditCustomFieldViewModel viewModel)
        {
            var customField = _context.CustomFields.Find(viewModel.Id);

            if (customField != null)
            {
                customField.Name = viewModel.Name;
                customField.Name_vi = viewModel.Name_vi;
                customField.Values_vi = viewModel.Values_vi;
                customField.Values = viewModel.Values;
                customField.DisplayOrder = viewModel.DisplayOrder;
                customField.IsEnabled = viewModel.IsEnabled;
                customField.IsAllowedToEditListValue = viewModel.IsAllowedToEditListValue;

                _context.CustomFields.AddOrUpdate(customField);

                _context.SaveChanges();

                // Update ticket categories
                var ticketCategoryCustomFields =
                    _context.TicketCategoryCustomField.Where(x => x.CustomFieldId == viewModel.Id);

                _context.TicketCategoryCustomField.RemoveRange(ticketCategoryCustomFields);

                foreach (var id in viewModel.TicketCategoryIds)
                {
                    var ticketCategoryCustomField = new TicketCategoryCustomField
                    {
                        CategoryId = id,
                        CustomFieldId = customField.Id
                    };

                    _context.TicketCategoryCustomField.Add(ticketCategoryCustomField);
                }

                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            var customFieldToDelete = new CustomField
            {
                Id = id
            };

            _context.CustomFields.Attach(customFieldToDelete);
            _context.CustomFields.Remove(customFieldToDelete);

            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}