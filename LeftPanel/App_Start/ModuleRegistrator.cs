using System.Reflection;
using System.Web.Mvc;
using Fanex.CustomerSetting.Module.Core.Web.App_Start;

namespace Fanex.ExternalAccount.Module.LeftPanel.App_Start
{
    public class ModuleRegistrator : AppAreaRegistrator 
    {
        protected override string ControllerName
        {
            get
            {
                return "LeftPanel";
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
                AreaName + "/" + ControllerName + "/{controller}/{action}/{id}/{uplineId}/{pageIndex}",
                new { action = "Index", id = UrlParameter.Optional, uplineId = UrlParameter.Optional, pageIndex = UrlParameter.Optional },
                new[] { ControllerNameSpace });
        }       
    }
}