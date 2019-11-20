namespace Fanex.ExternalAccount.ActivityTracking.Web
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.ExternalAccount.ActivityTracking.Configurations;
    using Fanex.ExternalAccount.ActivityTracking.LogEngines;
    using Fanex.Logging;
    using Fanex.Logging.Extensions;

    public static class LoggingConfig
    {
        [ThreadStatic]
        private static ExceptionHandlerAdapter exceptionHandler;

        [ThreadStatic]
        private static LogManagerAdapter logManager;

        [ThreadStatic]
        private static LoggingConfiguration loggingConfiguration;

        public static ExceptionHandlerAdapter ExceptionHandler
        {
            get => exceptionHandler ?? new ExceptionHandlerAdapter();
            set => exceptionHandler = value;
        }

        public static LogManagerAdapter LogManager
        {
            get => logManager ?? new LogManagerAdapter();
            set => logManager = value;
        }

        public static LoggingConfiguration LoggingConfiguration
        {
            get => loggingConfiguration ?? new LoggingConfiguration();
            set => loggingConfiguration = value;
        }

        public static void Init(IEnumerable<string> sensitiveInfo = null, Func<Exception, IDictionary<string, object>> getDbExceptionInfo = null)
        {
            var libs = AppSettings.LogLibs.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var logEngines = libs.Select(CreateLogLib).Where(engine => engine != null);
            var logDefaultCategory = AppSettings.LoggingDefaultCategory;

            LogManager.SetDefaultLogCategory(logDefaultCategory).Use(logEngines);

            var regexSensitiveInfo = GetRegexToHideSensitiveInfo(sensitiveInfo ?? Enumerable.Empty<string>());
            LoggingConfiguration = LoggingConfiguration.GetHttpHeaderInfo().GetHttpSessionInfo().HideSensitiveInfo(regexSensitiveInfo);
            if (getDbExceptionInfo != null)
            {
                LoggingConfiguration = LoggingConfiguration.UseGetDbExceptionInfo(getDbExceptionInfo);
            }

            ExceptionHandler.UseConfig(LoggingConfiguration);
        }

        public static ILogEngine CreateLogLib(string libName)
        {
            var type = Type.GetType($"Fanex.ExternalAccount.ActivityTracking.LogEngines.{libName}LogEngine");
            if (type == null)
            {
                return null;
            }

            var engine = Activator.CreateInstance(type) as LogEngineFactory;

            return engine?.Create();
        }

        public static List<string> GetRegexToHideSensitiveInfo(IEnumerable<string> sensitiveInfo)
        {
            var result = new List<string>();
            sensitiveInfo?.ToList().ForEach(info =>
            {
                result.Add($"(?:{info}=)(?<paramValue>[^&|^\\s]+)");
                result.Add($"(\"{info}\":)(?<paramValue>[^,|^\r]+)");
            });

            return result;
        }
    }
}
