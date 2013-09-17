// --------------------------------------------------------------------------
// <copyright file="HomeController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// <summary>
// Security code controller
// </summary>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2013.9.10">Created</change>
// </history>

namespace Fanex.ExternalAccount.SecurityCode.Controllers
{
    using System.Web.Mvc;
    using Fanex.ExternalAccount.Core.Base;
    using Fanex.ExternalAccount.Core.Context;
    using Fanex.ExternalAccount.DTO;

    /// <summary>
    /// Home controller.
    /// </summary>
    public class HomeController : BaseController
    {
        [Authorization(Function = eFunctions.EditCustomer, Policy = EPolicy.Read)]
        public ActionResult Index(int id, string groupName)
        {
            return this.PartialViewBase("~/Areas/CustomerSetting/Views/SecurityCode/Index.cshtml");
        }
    }
}