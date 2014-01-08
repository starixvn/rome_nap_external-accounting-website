// --------------------------------------------------------------------------
// <copyright file="AjaxRedirectResult.cs" company="Nexcel Solutions Vietnam">
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
    public class AjaxRedirectResult : RedirectResult
    {
        public AjaxRedirectResult(string url, ControllerContext controllerContext)
            : base(url)
        {
            ExecuteResult(controllerContext);
        }

        /// <summary>
        /// Overrides execute result action.
        /// </summary>
        /// <param name="context">ControllerContext object.</param>
        public override void ExecuteResult(ControllerContext context)
        {
            if (context.RequestContext.HttpContext.Request.IsAjaxRequest())
            {
                JavaScriptResult result = new JavaScriptResult()
                {
                    Script = "try{history.pushState(null,null,window.location.href);}catch(err){}window.location.replace('" + 
                    UrlHelper.GenerateContentUrl(this.Url, context.HttpContext) + "');"
                };

                result.ExecuteResult(context);
            }
            else
            {
                base.ExecuteResult(context);
            }
        }
    }
}