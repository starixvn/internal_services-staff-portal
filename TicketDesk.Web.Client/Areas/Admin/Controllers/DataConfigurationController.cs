namespace TicketDesk.Web.Client.Areas.Admin.Controllers
{
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

            if (dataConfigurationViewModel.SelectedFieldId != 0)
            {
                dataConfigurationViewModel.CustomFieldListValues =
                    _dataConfigurationService.GetEditableListValues(dataConfigurationViewModel.SelectedFieldId);
            }

            return View(viewModel);
        }
    }
}