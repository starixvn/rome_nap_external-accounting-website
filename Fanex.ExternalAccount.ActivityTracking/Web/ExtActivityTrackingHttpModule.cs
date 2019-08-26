namespace Fanex.ExternalAccount.ActivityTracking.Web
{
    using Fanex.ActivityTracking.Web;
    using Fanex.ExternalAccount.ActivityTracking.Models;
    using Fanex.ExternalAccount.ActivityTracking.Services;

    public class ExtActivityTrackingHttpModule : ActivityTrackingHttpModule
    {
        public ExtActivityTrackingHttpModule()
             : base(new ExtUserSession(), new RequestAnalyzer(), new JsonConverter())
        {
        }
    }
}
