// --------------------------------------------------------------------------
// <copyright file="ModuleRegistrator.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2013.9.17">Created</change>
// </history>

namespace Fanex.ExternalAccount.Module.LeftPanel.App_Start
{
    using System.Reflection;
    using System.Web.Mvc;
    using Fanex.CustomerSetting.Module.Core.Web.App_Start;

    /// <summary>
    /// Registers area for security code module.
    /// </summary>
    public class ModuleRegistrator : AppAreaRegistrator 
    {
        protected override string ControllerName
        {
            get
            {
                return "SecurityCode";
            }
        }

        protected override string ControllerNameSpace
        {
            get
            {
                var controllerNameSpace = Assembly.GetExecutingAssembly().GetName().Name + ".Controllers";
                return controllerNameSpace;
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                AreaName + "_" + ControllerName,
                AreaName + "/" + ControllerName + "/{controller}/{action}/{id}/{groupName}",
                new { action = "Index", id = UrlParameter.Optional, groupName = UrlParameter.Optional },
                new[] { ControllerNameSpace });
        }       
    }
}