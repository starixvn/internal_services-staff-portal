namespace TicketDesk.Web.Client.Areas.Admin.Controllers
{
    using System.Linq;
    using System.Web.Mvc;
    using Domain.Infrastructure;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;
    using Models.DataConfiguration;
    using Service.Interface;

    [NapAuth(FunctionKey = FunctionKeys.DataConfiguration, Policy = Constant.Policy.Read)]
    public class DataConfigurationController : Controller
    {
        private readonly IDataConfigurationService _dataConfigurationService;

        public DataConfigurationController(IDataConfigurationService dataConfirgurationService)
        {
            _dataConfigurationService = dataConfirgurationService;
        }

        public ActionResult Index(DataConfigurationViewModel viewModel)
        {
            var dataConfigurationViewModel = viewModel ?? new DataConfigurationViewModel();

            dataConfigurationViewModel.EditableCustomFields = _dataConfigurationService.GetEditableCustomFields();

            if (dataConfigurationViewModel.SelectedFieldId == 0)
            {
                dataConfigurationViewModel.SelectedFieldId =
                    dataConfigurationViewModel.EditableCustomFields.First().CustomFieldId;
            }

            dataConfigurationViewModel.CustomFieldListValues =
                _dataConfigurationService.GetEditableListValues(dataConfigurationViewModel.SelectedFieldId);

            return View(viewModel);
        }

        public ActionResult AddCustomFieldListValue(int customFieldId, string modalTitle)
        {
            var addCustomFieldListValueViewModel = new AddCustomFieldListValueViewModel
            {
                CustomFieldId = customFieldId,
                ModalTitle = modalTitle
            };

            return PartialView("_AddCustomFieldListValue", addCustomFieldListValueViewModel);
        }

        [AcceptVerbs("Get", "Post")]
        public ActionResult IsDisplayNameViUnique(int customFieldId, string displayNameVi)
        {
            var result = _dataConfigurationService.IsCustomFieldDisplayNameViUnique(customFieldId, displayNameVi);
            return Json(result);
        }

        [AcceptVerbs("Get", "Post")]
        public ActionResult IsDisplayNameEnUnique(int customFieldId, string displayNameEn)
        {
            var result = _dataConfigurationService.IsCustomFieldDisplayNameEnUnique(customFieldId, displayNameEn);
            return Json(result);
        }
    }
}