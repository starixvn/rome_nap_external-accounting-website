// --------------------------------------------------------------------------
// <copyright file="HomeController.cs" company="">
// Copyright (c) . All rights reserved.
// </copyright>
// <summary>
// For current session info
// </summary>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2012.12.14">Created</change>
// </history>

using System;
using System.Linq;
using System.Web.Mvc;
using Fanex.CustomerSetting.Security.Authenticate;
using Fanex.ExternalAccount.Core.Base;
using Fanex.ExternalAccount.Core.Context;
using Fanex.ExternalAccount.Service.Impl.Login;
using Fanex.ExternalAccount.Utilities;
using Fanex.Logging.Extensions.Web;
using ONEbook.UM.Services;
using ONEbook.UM.Services.Impl;

namespace Fanex.ExternalAccount.Controllers.Home
{
    /// <summary>
    /// Home controller.
    /// </summary>
    public class HomeController : BaseController
    {
        private IMaintenanceService _maintenanceService = new MaintenanceService();
        
        /// <summary>
        /// Home page.
        /// </summary>
        /// <returns>ActionResult object.</returns>
        [ExternalAccountAuthenticate(Function = "Home", Policy = AuthenticateAction.AuthenticationType.Read)]
        public ActionResult Index()
        {
            if (_maintenanceService.IsUnderMaintenance())
            {
                return RedirectToAction("Offline", "Account");
            }

            if (!CurrentSession.IsLogin && !CurrentContext.Context.Request.RawUrl.ToLower().Contains("login"))
            {
                return RedirectToAction("Login", "Account");
            }

            if (CurrentSession.IsLogin && CurrentContext.Context.Request.RawUrl.ToLower().Contains("login"))
            {
                var group = CurrentSession.IsInternalAdmin ? new LoginService().GroupList().First().GroupName : string.Empty;

                return Redirect("~/Home/Index/" + group);
            }

            return ViewBase();
        }

        public ActionResult TestLog()
        {
            ExceptionHandler.Handle(new Exception("Testing log"));

            return ViewBase();
        }
    }
}