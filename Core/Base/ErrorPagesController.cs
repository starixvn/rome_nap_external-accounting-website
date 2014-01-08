// --------------------------------------------------------------------------
// <copyright file="ErrorPagesController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2014.1.8">Created</change>
// </history>

namespace Fanex.ExternalAccount.Core.Base
{
    using System.Web.Mvc;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ErrorPagesController : Controller
    {
        public ActionResult AjaxGlobalError(string newActionName)
        {
            return new AjaxRedirectResult(Url.Action(newActionName), this.ControllerContext);
        }
    }
}