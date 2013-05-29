// -----------------------------------------------------------------------
// <copyright file="LoggingService.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Fanex.Logging;

namespace Fanex.Puffin.Core.Services.Impl
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class LoggingHandler
    {
        public static void Log(Exception ex)
        {
           LogService.Error(ex.Message, ex);
        }

        public static void Log(string message = "", LogType logType = LogType.Error)
        {
            switch (logType)
            {
                case LogType.Info:
                    LogService.Info(message);
                    break;
                case LogType.Error:
                    LogService.Error(message);
                    break;
            }
        }
    }

    public enum LogType: int
    {
        Info = 1,
        Error = 2
    }
        
}
