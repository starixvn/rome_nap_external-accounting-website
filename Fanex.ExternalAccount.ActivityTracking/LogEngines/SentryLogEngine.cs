namespace Fanex.ExternalAccount.ActivityTracking.LogEngines
{
    using System.Web;
    using Fanex.ExternalAccount.ActivityTracking.Configurations;
    using Fanex.Logging;
    using Fanex.Logging.Sentry.RavenClient;

    public class SentryLogEngine : LogEngineFactory
    {
        public ILogEngine Create()
        {
            var sentryOptions = new SentryEngineOptions
            {
                Dsn = new SharpRaven.Dsn(AppSettings.SentryBackendUrl),
                FrontEndDsn = new SharpRaven.Dsn(AppSettings.SentryUrl)
            };

            return new SentryLogging(
                sentryOptions,
                $"{HttpRuntime.AppDomainAppVirtualPath.TrimEnd(new[] { '/' })}{AppSettings.RavenLibPath}");
        }
    }
}
