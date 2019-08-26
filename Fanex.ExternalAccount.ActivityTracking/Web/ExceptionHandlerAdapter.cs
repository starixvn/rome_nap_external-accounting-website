namespace Fanex.ExternalAccount.ActivityTracking.Web
{
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Web;

    public class ExceptionHandlerAdapter
    {
        public virtual LoggingConfiguration UseConfig(LoggingConfiguration loggingConfiguration)
            => ExceptionHandler.UseConfig(loggingConfiguration);
    }
}
