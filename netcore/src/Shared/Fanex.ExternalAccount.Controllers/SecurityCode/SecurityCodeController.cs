namespace Fanex.ExternalAccount.Controllers.SecurityCode
{
    using Fanex.CustomerSetting.Security.Authenticate;
    using Fanex.ExternalAccount.Core.Configurations.SecurityCode;
    using Fanex.ExternalAccount.Core.Context;
    using System;
    using Microsoft.AspNetCore.Mvc;

    [ExternalAccountAuthenticate(Policy = AuthenticateAction.AuthenticationType.Read, IsAjaxRequest = false)]
    public class SecurityCodeController : Controller
    {
        private readonly CurrentSession currentSession;

        public SecurityCodeController()
        {
            currentSession = new CurrentSession();
        }

        [HttpGet]
        public IActionResult Index(string ReturnUrl)
        {
            ViewBag.UserName = currentSession.UserName;
            ViewBag.IsExternalSite = ReturnUrl.Contains("/updateCustomerSetting/updateSetting") 
                || ReturnUrl.Contains("/updateCustomerSetting/updateMultipleSetting");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Verify(string txtSecurityCode, string txtSecretKey)
        {
            string userSecurityCode = SecurityCodeManager.GetSecurityCode(currentSession.UserName);
            string rawSecurityCode = Encryptor.Decrypt(txtSecurityCode, txtSecretKey);
            if (userSecurityCode.Equals(rawSecurityCode, StringComparison.CurrentCulture))
            {
                currentSession.UpdateLastActiveTime();
                currentSession.SCFailedCounter = 0;

                return Ok(new
                {
                    StatusCode = 1
                });
            }

            return Ok(new
            {
                StatusCode = 0,
                FailedCounter = ++currentSession.SCFailedCounter
            });
        }
    }
}