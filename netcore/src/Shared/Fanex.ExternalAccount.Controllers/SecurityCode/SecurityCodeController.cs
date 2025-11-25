namespace Fanex.ExternalAccount.Controllers.SecurityCode
{
    using Fanex.CustomerSetting.Security.Authenticate;
    using Fanex.ExternalAccount.Core.Configurations.SecurityCode;
    using Fanex.ExternalAccount.Core.Context;
    using System;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;

    [ExternalAccountAuthenticate(Policy = AuthenticateAction.AuthenticationType.Read, IsAjaxRequest = false)]
    public class SecurityCodeController : Controller
    {
        private readonly ICurrentSession currentSession;
        private readonly SecurityCodeManager _securityCodeManager;

        public SecurityCodeController(IConfiguration configuration, ICurrentSession currentSession)
        {
            this.currentSession = currentSession;
            _securityCodeManager = new SecurityCodeManager(configuration);
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
            string userSecurityCode = _securityCodeManager.GetSecurityCode(currentSession.UserName);
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