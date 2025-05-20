namespace Fanex.ExternalAccount.Core.App_Start
{
    using System.Collections.Generic;
    using System.Web.Mvc;

    public class AppAreaRegistrator : AreaRegistration
    {
        private List<string> _controllers = new List<string>();

        public override string AreaName
        {
            get
            {
                return "CustomerSetting";
            }
        }

        protected virtual string ModuleName { get { return string.Empty; } }

        protected virtual string ModuleNameSpace { get { return string.Empty; } }


        protected void RegisterController(string controllerName)
        {
            if (string.IsNullOrEmpty(controllerName))
                return;
            _controllers.Add(controllerName);
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            if (!string.IsNullOrWhiteSpace(this.ModuleName) && !string.IsNullOrWhiteSpace(this.ModuleNameSpace))
            {
                var areaName = AreaName + "_" + ModuleName;
                if (context.Routes[areaName] != null)
                    return;
                context.MapRoute(
                    areaName,
                    AreaName + "/" + ModuleName + "/{controller}/{action}",
                    new { action = "Index" },
                    new[] { ModuleNameSpace });
            }
        }

        protected void RegisterController(AreaRegistrationContext context)
        {
        }
    }
}
