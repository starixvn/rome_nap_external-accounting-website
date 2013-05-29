// --------------------------------------------------------------------------
// <copyright file="CommonController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Ricky.Nguyen" date="2013.1.4">Created</change>
// </history>

using System.Web.Mvc;
using System.Diagnostics.CodeAnalysis;
using System.Web.Security;
using Fanex.NAP.Core.Service.Impl;
using Fanex.NAP.Core.Service.Interface;
using Fanex.Puffin.Core.Services.Impl;
using Fanex.Puffin.Core.Services.Interface;

namespace Fanex.Puffin.Website.Areas.UserLogin.Controllers
{
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented")]
    public class CommonController : Controller
    {
        private readonly IMaintainanceService _maintenanceService;
        private ICoreService _coreService;

        public CommonController()
            : this(null)
        {
        }

        public CommonController(IMaintainanceService maintenanceService)
        {
            _maintenanceService = maintenanceService ?? new MaintainanceService();
            _coreService = new CoreService();
        }

        public ActionResult Offline()
        {
            var downTime = _maintenanceService.GetDowntimeInfo();

            if (!downTime.IsDowntime)
            {
                FormsAuthentication.RedirectToLoginPage();
            }

            ViewBag.UmtFrom = downTime.From;
            ViewBag.UmtTo = downTime.To;

            return View();
        }

        public ActionResult AccessDenied()
        {
            return View("AccessDenied");
        }

       
    }
}
