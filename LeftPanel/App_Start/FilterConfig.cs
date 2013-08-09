using System.Web.Mvc;

namespace Fanex.ExternalAccount.Module.LeftPanel.App_Start
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}