namespace TicketDesk.Web.Client.Areas.Admin.Controllers
{
    using System.Web.Mvc;
    using Domain.Infrastructure;
    using Domain.Model;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;
    using Models.DataConfiguration;
    using Service.Interface;
    using Service.Interface.Admin;

    [NapAuth(FunctionKey = FunctionKeys.DataConfiguration, Policy = Constant.Policy.Read)]
    public class DataConfigurationController : Controller
    {
        private readonly IDataConfigurationService _dataConfigurationService;

        public DataConfigurationController(IDataConfigurationService dataConfirgurationService)
        {
            _dataConfigurationService = dataConfirgurationService;
        }

        public ActionResult Index()
        {
            var dataConfigurationViewModel = new DataConfigurationViewModel
            {
                EditableCustomFields = _dataConfigurationService.GetEditableCustomFields()
            };

            return View(dataConfigurationViewModel);
        }

        public ActionResult ListCustomFieldListValue(int selectedFieldId, string modalTitle)
        {
            ListCustomFieldListValueViewModel viewModel = new ListCustomFieldListValueViewModel
            {
                CustomFieldListValues = _dataConfigurationService.GetEditableListValues(selectedFieldId),
                SelectedFieldId = selectedFieldId,
                ModalTitle = modalTitle
            };

            return PartialView("_ListCustomFieldListValue", viewModel);
        }

        public ActionResult CreateCustomFieldListValue(int customFieldId, string modalTitle)
        {
            var createCustomFieldListValueViewModel = new CreateCustomFieldListValueViewModel
            {
                CustomFieldId = customFieldId,
                ModalTitle = modalTitle
            };

            return PartialView("_CreateCustomFieldListValue", createCustomFieldListValueViewModel);
        }

        [HttpPost]
        public ActionResult CreateAndContinueCustomFieldListValue(CreateCustomFieldListValueViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                CustomFieldListValue customFieldListValue = new CustomFieldListValue
                {
                    CustomFieldId = viewModel.CustomFieldId,
                    DisplayName_vi = viewModel.DisplayName_vi,
                    DisplayName = viewModel.DisplayName,
                    IsActive = viewModel.IsActive
                };

                _dataConfigurationService.CreateCustomFieldListValue(customFieldListValue);
            }

            var newViewModel = new CreateCustomFieldListValueViewModel
            {
                CustomFieldId = viewModel.CustomFieldId,
                ModalTitle = viewModel.ModalTitle
            };

            return PartialView("_CreateCustomFieldListValue", newViewModel);
        }

        [HttpPost]
        public ActionResult CreateCustomFieldListValue(CreateCustomFieldListValueViewModel viewModel)
        {
            var result = false;

            if (ModelState.IsValid)
            {
                CustomFieldListValue customFieldListValue = new CustomFieldListValue
                {
                    CustomFieldId = viewModel.CustomFieldId,
                    DisplayName_vi = viewModel.DisplayName_vi,
                    DisplayName = viewModel.DisplayName,
                    IsActive = viewModel.IsActive
                };

                result = _dataConfigurationService.CreateCustomFieldListValue(customFieldListValue);
            }

            return Json(result);
        }

        public ActionResult EditCustomFieldListvalue(int id)
        {
            CustomFieldListValue customFieldListValue = _dataConfigurationService.GetEditableListValue(id);

            return PartialView("_EditCustomFieldListValue", customFieldListValue);
        }

        [HttpPost]
        public ActionResult EditCustomFieldListValue(CustomFieldListValue customFieldListValue)
        {
            var result = false;
            if (ModelState.IsValid)
            {
                result = _dataConfigurationService.EditCustomFieldListValue(customFieldListValue);
            }

            return Json(result);
        }

        [AcceptVerbs("Get", "Post")]
        public ActionResult IsDisplayNameViUnique(CreateCustomFieldListValueViewModel viewModel)
        {
            var result = _dataConfigurationService.IsCustomFieldDisplayNameViUnique(viewModel.CustomFieldId, viewModel.DisplayName_vi);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs("Get", "Post")]
        public ActionResult IsDisplayNameEnUnique(int customFieldId, string displayNameEn)
        {
            var result = _dataConfigurationService.IsCustomFieldDisplayNameEnUnique(customFieldId, displayNameEn);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}