namespace Fanex.ExternalAccount.Module.LeftPanel.App_Start
{
    using System.Reflection;
    using System.Web.Mvc;
    using Fanex.ExternalAccount.Core.App_Start;

    public class ModuleRegistrator : AppAreaRegistrator
    {
        protected static string ControllerName => "LeftPanel";

#pragma warning disable S3902 // "Assembly.GetExecutingAssembly" should not be called
        protected static string ControllerNameSpace => Assembly.GetExecutingAssembly().GetName().Name + ".Controllers";
#pragma warning restore S3902 // "Assembly.GetExecutingAssembly" should not be called

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                AreaName + "_" + ControllerName,
                AreaName + "/" + ControllerName + "/{controller}/{action}/{id}/{uplineId}/{pageIndex}",
                new
                {
                    action = "Index",
                    id = UrlParameter.Optional,
                    uplineId = UrlParameter.Optional,
                    pageIndex = UrlParameter.Optional
                },
                new[] {ControllerNameSpace});
        }
    }
}