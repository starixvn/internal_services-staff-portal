namespace TicketDesk.Web.Client.Controllers
{
    using Models;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    public class IdUploaderController : Controller
    {
        // GET: IdUploader
        public ActionResult Index()
        {
            IdUploaderViewModel viewModel = new IdUploaderViewModel();
            return View(viewModel);
        }

        //[NapAuth(FunctionKey = FunctionKeys., Policy = Constant.Policy.Read)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> NewCustomer(IdUploaderViewModel viewModel)
        {

            return View("Index", viewModel);
        }
    }
}