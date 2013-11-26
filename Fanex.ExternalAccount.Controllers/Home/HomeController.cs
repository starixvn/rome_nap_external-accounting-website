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
    using System.Web.Mvc;
    using Fanex.CustomerSetting.Security.Authenticate;
    using Fanex.ExternalAccount.Core.Base;
    using Fanex.ExternalAccount.Core.Context;

    /// <summary>
    /// Home controller.
    /// </summary>
    public class HomeController : BaseController
    {
        [ExternalAccountAuthenticate(Function = "Home", Policy = AuthenticateAction.AuthenticationType.Read)]
        public ActionResult Index()
        {
            return this.ViewBase();
        }

        public ActionResult TestLog()
        {
            Logging.LogService.Error("Testing, this is an exception!!!", "ExternalAccount");

            return this.ViewBase();
        }
    }
}