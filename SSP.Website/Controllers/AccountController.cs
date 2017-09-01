namespace SSP.Website.Controllers
{
    using Fanex.Common;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using SSP.Core.Constants;
    using SSP.Core.Resources;
    using SSP.Models.Account;
    using SSP.Services;
    using SSP.Website.ViewModels.Account;
    using System;
    using System.Globalization;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;

    public class AccountController : BaseController
    {
        private readonly IAccountService accountService;
        private readonly HashAlgorithm hashAlgorithm;

        public AccountController(IAccountService accountService)
        {
            this.accountService = accountService ?? new AccountService();
            hashAlgorithm = new SHA256Managed();
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = "")
        {
            var model = new LoginViewModel
            {
                ReturnUrl = returnUrl
            };

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid || model == null)
            {
                return View(model);
            }

            if (!string.IsNullOrWhiteSpace(model.Username) && !string.IsNullOrWhiteSpace(model.Password))
            {
                UserSession user = await accountService.GetUserLoginAsync(model.Username);

                if (user != null && !string.IsNullOrWhiteSpace(model.Password))
                {
                    string hashPassword = GetHash(user.Password);

                    if (model.Password == hashPassword)
                    {
                        HttpContext.Session.Set<UserSession>(Constants.CurrentSessionKey, user);

                        Response.Redirect(Url.IsLocalUrl(model.ReturnUrl) ? model.ReturnUrl : "Home/Index");
                    }
                    else
                    {
                        ViewBag.Message = Messages.LogIn_WrongUsnOrPwd;
                    }
                }
                else
                {
                    ViewBag.Message = Messages.LogIn_WrongUsnOrPwd;
                }
            }
            else
            {
                ViewBag.Message = Messages.LogIn_MissingUsnOrPwd;
            }

            return View(model);
        }

        public IActionResult Logout()
        {
            return View();
        }

        private string GetHash(string input)
        {
            var encrypted = string.Empty;

            if (!string.IsNullOrWhiteSpace(input))
            {
                var hashData = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));

                encrypted = BitConverter.ToString(hashData).Replace("-", string.Empty).ToLower(CultureInfo.InvariantCulture);
            }

            return encrypted;
        }
    }
}