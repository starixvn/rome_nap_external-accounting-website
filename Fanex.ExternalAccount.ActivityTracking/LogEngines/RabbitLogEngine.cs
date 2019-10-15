namespace Fanex.ExternalAccount.ActivityTracking.LogEngines
{
    using Fanex.ExternalAccount.ActivityTracking.Configurations;
    using Fanex.Logging;
    using Fanex.Logging.RabbitMQ;

    public class RabbitLogEngine : LogEngineFactory
    {
        public ILogEngine Create()
            => new RabbitMQLogging(AppSettings.RabbitMQConnectionStrings);
    }
}
