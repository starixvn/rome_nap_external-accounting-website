namespace Fanex.ExternalAccount.ActivityTracking.LogEngines
{
    using Fanex.Logging;

    public interface LogEngineFactory
    {
        ILogEngine Create();
    }
}
