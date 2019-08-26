namespace Fanex.ExternalAccount.ActivityTracking.Web
{
    using Fanex.Logging;

    public class LogManagerAdapter : LogManager
    {
        public new virtual LogManager SetDefaultLogCategory(string defaultCategory)
            => LogManager.SetDefaultLogCategory(defaultCategory);
    }
}
