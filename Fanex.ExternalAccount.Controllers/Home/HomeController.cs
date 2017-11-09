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
using Fanex.CustomerSetting.Module.Core.Web;
using Fanex.CustomerSetting.Security.Authenticate;
using Fanex.ExternalAccount.Core.Context;
using Fanex.ExternalAccount.Service.Impl.Login;
using Fanex.ExternalAccount.Utilities;
using ONEbook.UM.Services;
using ONEbook.UM.Services.Impl;

namespace Fanex.ExternalAccount.Controllers.Home
{
    /// <summary>
    /// Home controller.
    /// </summary>
    public class HomeController : Controller
    {
        private readonly IMaintenanceService _maintenanceService = new MaintenanceService();
        private readonly ICurrentSession _currentSession = new Core.Context.CurrentSession();
        private readonly ICurrentContext _currentContext = new Core.Context.CurrentContext();

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

            if (!_currentSession.IsLogin && !_currentContext.Context.Request.RawUrl.ToLower().Contains("login"))
            {
                return RedirectToAction("Login", "Account");
            }

            if (_currentSession.IsLogin && _currentContext.Context.Request.RawUrl.ToLower().Contains("login"))
            {
                var group = _currentSession.IsInternalAdmin ? new LoginService().GroupList().First().GroupName : string.Empty;

                return Redirect("~/Home/Index/" + group);
            }

            return View();
        }

        public ActionResult TestLog()
        {
            Logs.Error(new Exception("Testing log"));

            return View();
        }
    }
}