// --------------------------------------------------------------------------
// <copyright file="HomeController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// <summary>
// For current session info
// </summary>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2012.12.14">Created</change>
// </history>

namespace Fanex.ExternalAccount.Controllers.Home
{
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.CustomerSetting.Security.Authenticate;
    using Fanex.ExternalAccount.Core.Base;
    using Fanex.ExternalAccount.Core.Context;
    using Fanex.ExternalAccount.Service.Impl.Login;
    using Fanex.ExternalAccount.Utilities;

    /// <summary>
    /// Home controller.
    /// </summary>
    public class HomeController : BaseController
    {
        /// <summary>
        /// Home page.
        /// </summary>
        /// <returns>ActionResult object.</returns>
        [ExternalAccountAuthenticate(Function = "Home", Policy = AuthenticateAction.AuthenticationType.Read)]
        public ActionResult Index()
        {
            var maintenanceInfo = Config.GetMaintenanceInfo();

            if (maintenanceInfo.IsDownTime)
            {
                return this.RedirectToAction("Offline", "Account");
            }

            if (!CurrentSession.IsLogin && !CurrentContext.Context.Request.RawUrl.ToLower().Contains("login"))
            {
                return this.RedirectToAction("Login", "Account");
            }

            if (CurrentSession.IsLogin && CurrentContext.Context.Request.RawUrl.ToLower().Contains("login"))
            {
                var group = CurrentSession.IsInternalAdmin ? new LoginService().GroupList().First().GroupName : string.Empty;

                return this.Redirect("~/Home/Index/" + group);
            }

            return this.ViewBase();
        }

        public ActionResult TestLog()
        {
            Logging.LogService.Error("Testing, this is an exception!!!", "ExternalAccount");

            return this.ViewBase();
        }
    }
}