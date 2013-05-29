using System.Web.Mvc;
using System.Web.Optimization;
using Fanex.Puffin.Website.Areas.Report.Area_Start;

namespace Fanex.Puffin.Website.Areas.Report
{
    public class ReportAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Report";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            RouteConfig.RegisterRoutes(this.AreaName, context);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}