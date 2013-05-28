using System.Web.Mvc;
using System.Web.Optimization;
using Fanex.Puffin.Website.Areas.Admin.Area_Start;

namespace Fanex.Puffin.Website.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            RouteConfig.RegisterRoutes(this.AreaName, context);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}