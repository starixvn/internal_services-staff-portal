namespace SSP.Website.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using SSP.Models.Account;

    public class AccountController : Controller
    {
        [HttpGet]
        public ActionResult Login()
        {
            var model = new LoginModel();
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginModel model)
        {
            if (!ModelState.IsValid || model == null)
            {
                return View(model);
            }

            return View(model);
        }

        public IActionResult Logout()
        {
            return View();
        }
    }
}