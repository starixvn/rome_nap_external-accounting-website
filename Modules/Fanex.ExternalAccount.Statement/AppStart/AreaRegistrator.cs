namespace Fanex.ExternalAccount.Statement.AppStart
{
    using System.Web.Http;
    using System.Web.Mvc;
    using System.Web.Optimization;

    public class AreaRegistrator : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "NapStatement";
            }
        }

        public static void Register(HttpConfiguration config)
        {
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            RouteConfig.RegisterRoutes(AreaName, context);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
