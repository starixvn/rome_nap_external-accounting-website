// --------------------------------------------------------------------------
// <copyright file="BaseController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.10.12">Created</change>
//   <change who="Ricky.Nguyen" date="2012.11.14">Update Maintenance Time</change>
// </history>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Web.Mvc;
using Fanex.NAP.Core.Business;
using Fanex.NAP.Core.Configuration;
using Fanex.NAP.Core.Service.Impl;
using Fanex.NAP.Core.Service.Interface;
using Fanex.NAP.Core.Web;
using Fanex.NAP.Core.Web.Context;
using Fanex.NAP.Core.Web.Session;
using Fanex.Puffin.Core.Services.Impl;

namespace Fanex.Puffin.Core.Web.Controllers
{
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented")]
    public class BaseController : Controller
    {
        protected readonly ICurrentContext CurrentContext;

        protected readonly ICurrentSession CurrentSession;

        protected readonly IServiceContext ServiceContext;

        protected readonly string Environment = NapSettings.Settings.Environment;

        protected readonly int SiteId = NapSettings.Settings.SiteId;

        private int _issiteLogin = NapSettings.Settings.IsSiteLogin;

        protected ViewFlags _viewFlags;

        protected ViewFlags ViewFlags
        {
            get
            {
                return ViewBag.ViewFlags;
            }

            set
            {
                ViewBag.ViewFlags = this._viewFlags = value;
            }
        }

        private MenuService _menuService;

        public BaseController()
        {
            this.CurrentContext = new CurrentContext();
            ViewBag.Session = this.CurrentSession = new CurrentSession();
            this.ServiceContext = new ServiceContext
            {
                UserId = this.CurrentSession.UserId,
                SiteId = this.SiteId,
                Ip = this.CurrentContext.IP
            };

            ViewFlags = new ViewFlags
            {
                IsLive = this.Environment.Equals(Constant.LIVE),
                // IsNapAdminSite = _siteId == NapAdminId,
                HasEditRight = false
            };

            this._menuService = new MenuService();
        }

        /// <summary>
        /// Execute before each request, if Downtime then go to Downtime page
        /// Or not Authenticate will go to Login Page
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            AppSession.CurrentActionName = filterContext.ActionDescriptor.ActionName;
            base.OnActionExecuting(filterContext);
        }

        protected override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            // Get current working on menu for getting display name,... on webpage
            if (filterContext.Result is ViewResult && string.Equals(AppSession.CurrentActionName, "Index", StringComparison.OrdinalIgnoreCase))
            {
                var menuPath = MenuService.GetMenuPathFromRequestFilePath(filterContext.HttpContext.Request.FilePath, Request.ApplicationPath);
                string controllerName = (string)filterContext.RouteData.Values["controller"];
                AppSession.CurrentWorkingOnMenu = this._menuService.GetMenuByURL(menuPath);
            }
        }

        /// <summary>
        /// Indicates whether the current site allows users login directly.
        /// If "0", users must login through NAP Hub
        /// </summary>
        public int IsSiteLogin
        {
            get
            {
                return this._issiteLogin;
            }
        }

        protected override JsonResult Json(object data, string contentType, System.Text.Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new JsonResult()
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior,
                MaxJsonLength = Int32.MaxValue
            };
        }

        /// <summary>
        /// Get list actions of Controller
        /// </summary>
        /// <typeparam name="T">Controller</typeparam>
        /// <returns></returns>
        protected static List<string> GetActionListByController<T>()
        {
            var listAction = new List<string>();

            Type t = typeof(T);
            MethodInfo[] mi = t.GetMethods();
            foreach (MethodInfo m in mi)
            {
                if (m.IsPublic)
                    if (m.ReturnParameter != null && typeof(ActionResult).IsAssignableFrom(m.ReturnParameter.ParameterType))
                    {
                        listAction.Add(m.Name);
                    }
            }


            return listAction;

        }
    }
}
