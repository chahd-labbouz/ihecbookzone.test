using System;

namespace IHECBookzone.Desktop.Utils
{
    public interface ILogger
    {
        void Log(LogLevel level, string message);
        void LogException(Exception ex, string context = null);
    }

    public class LoggerWrapper : ILogger
    {
        public void Log(LogLevel level, string message) => Logger.Log(level, message);
        
        public void LogException(Exception ex, string context = null) => 
            Logger.LogException(ex, context);
    }
} 