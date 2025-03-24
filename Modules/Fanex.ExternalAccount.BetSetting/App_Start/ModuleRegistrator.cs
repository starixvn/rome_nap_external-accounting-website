namespace Fanex.CustomerSetting.Module.BetSetting.App_Start
{
    using System.Reflection;
    using System.Web.Mvc;
    using Fanex.ExternalAccount.Core.App_Start;

    public class ModuleRegistrator : AppAreaRegistrator
    {
        protected override string ModuleName => "BetSetting";

        protected override string ModuleNameSpace
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
                AreaName + "_" + ModuleName,
                AreaName + "/" + ModuleName + "/{controller}/{action}",
                new
                {
                    action = "Index"
                },
                new[] { ModuleNameSpace });
        }
    }
}