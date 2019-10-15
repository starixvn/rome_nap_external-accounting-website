namespace Fanex.ExternalAccount.ActivityTracking.Services
{
    using Fanex.ActivityTracking.Web;

    public class RequestAnalyzer : BaseRequestAnalyzer
    {
        public RequestAnalyzer() : base(new JsonConverter())
        {
        }
    }
}
