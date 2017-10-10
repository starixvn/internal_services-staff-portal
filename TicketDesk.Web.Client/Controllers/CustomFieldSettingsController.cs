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
                CustomFields = _domainContext.CustomFields
            };

            foreach (CustomField customField in viewModel.CustomFields)
            {
                var test = _domainContext.TicketCategoryCustomField.Where(x => x.CustomFieldId == customField.Id)
                    .Select(x => x.CategoryId);

                customField.TicketCategories = _domainContext.TicketCategories.Where(x => test.Contains(x.CategoryId)).ToList();

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
            //CustomField customField = new CustomField
            //{
            //    Name = viewModel.Name,
            //    CustomFieldType = new CustomFieldType { Id = viewModel.CustomFieldTypeId },
            //    Values = viewModel.Values
            //};

            //_domainContext.CustomFields.Add(customField);

            //_domainContext.TicketCategories
            //    .First(x => x.CategoryId == viewModel.TicketCategoryId).CustomFields
            //    .Add(customField);

            //_domainContext.CustomFieldTypes
            //    .First(x => x.Id == viewModel.CustomFieldTypeId).CustomFields
            //    .Add(customField);

            //_domainContext.SaveChanges();

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