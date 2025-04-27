using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace IHECBookzone.Desktop.Utils
{
    public enum LogLevel
    {
        Debug,
        Info,
        Warning,
        Error,
        Critical
    }

    public static class Logger
    {
        private static readonly string _logFilePath;
        private static readonly object _lockObj = new object();
        private static readonly Queue<string> _logQueue = new Queue<string>();
        private static bool _isProcessingQueue = false;
        private static readonly int _maxQueueSize = 100;
        private static readonly TimeSpan _flushInterval = TimeSpan.FromSeconds(5);
        private static DateTime _lastFlushTime = DateTime.Now;
        
        // Event that UI can subscribe to for showing notifications
        public static event EventHandler<LogMessageEventArgs> LogMessageAdded;

        static Logger()
        {
            try
            {
                // Create logs directory in application folder
                string appDataFolder = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "IHECBookzone");
                
                if (!Directory.Exists(appDataFolder))
                {
                    Directory.CreateDirectory(appDataFolder);
                }

                string logsFolder = Path.Combine(appDataFolder, "Logs");
                if (!Directory.Exists(logsFolder))
                {
                    Directory.CreateDirectory(logsFolder);
                }

                // Create log file with date in name
                string fileName = $"app_log_{DateTime.Now:yyyy-MM-dd}.log";
                _logFilePath = Path.Combine(logsFolder, fileName);
                
                // Write startup entry
                Log(LogLevel.Info, "Application started");
            }
            catch (Exception ex)
            {
                // Fallback to desktop if we can't create the proper location
                _logFilePath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    "ihec_bookzone_log.txt");
                
                // Log startup error directly to the file
                try
                {
                    File.AppendAllText(_logFilePath, FormatLogMessage(LogLevel.Error, $"Failed to initialize logger: {ex.Message}"));
                }
                catch
                {
                    // Last resort: can't even write to desktop
                    // Nothing more we can do
                }
            }
        }

        public static void Log(LogLevel level, string message)
        {
            if (string.IsNullOrEmpty(message))
                return;

            string formattedMessage = FormatLogMessage(level, message);
            
            // Add to queue for batch processing
            lock (_lockObj)
            {
                _logQueue.Enqueue(formattedMessage);
                
                // Trim queue if it gets too large
                while (_logQueue.Count > _maxQueueSize)
                {
                    _logQueue.Dequeue();
                }
                
                // Process queue if not already processing and enough time has passed or queue is getting full
                if (!_isProcessingQueue && 
                    (DateTime.Now - _lastFlushTime > _flushInterval || _logQueue.Count >= _maxQueueSize/2))
                {
                    _isProcessingQueue = true;
                    Task.Run(ProcessLogQueueAsync);
                }
            }
            
            // Trigger event for UI notification
            if (level >= LogLevel.Warning)
            {
                Application.Current?.Dispatcher?.Invoke(() =>
                {
                    LogMessageAdded?.Invoke(null, new LogMessageEventArgs(level, message));
                });
            }
        }
        
        public static void LogException(Exception ex, string context = null)
        {
            string message = string.IsNullOrEmpty(context) 
                ? $"Exception: {ex.Message}" 
                : $"Exception in {context}: {ex.Message}";
                
            Log(LogLevel.Error, message);
            
            // Add stack trace at debug level
            Log(LogLevel.Debug, $"Stack trace: {ex.StackTrace}");
            
            // Log inner exception if present
            if (ex.InnerException != null)
            {
                Log(LogLevel.Debug, $"Inner exception: {ex.InnerException.Message}");
            }
        }

        private static string FormatLogMessage(LogLevel level, string message)
        {
            return $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level.ToString().ToUpper()}] {message}{Environment.NewLine}";
        }
        
        private static async Task ProcessLogQueueAsync()
        {
            try
            {
                List<string> messages = new List<string>();
                
                // Grab all current messages from the queue
                lock (_lockObj)
                {
                    while (_logQueue.Count > 0)
                    {
                        messages.Add(_logQueue.Dequeue());
                    }
                    
                    _lastFlushTime = DateTime.Now;
                }
                
                if (messages.Count > 0)
                {
                    // Write all messages to file
                    await File.AppendAllTextAsync(_logFilePath, string.Concat(messages));
                }
            }
            catch (Exception ex)
            {
                // Try direct write as a fallback
                try
                {
                    File.AppendAllText(_logFilePath, 
                        FormatLogMessage(LogLevel.Critical, $"Failed to process log queue: {ex.Message}"));
                }
                catch
                {
                    // Nothing more we can do
                }
            }
            finally
            {
                lock (_lockObj)
                {
                    _isProcessingQueue = false;
                    
                    // If more items were added while processing, start again
                    if (_logQueue.Count > 0)
                    {
                        _isProcessingQueue = true;
                        Task.Run(ProcessLogQueueAsync);
                    }
                }
            }
        }
    }
    
    // Event args for notifying UI
    public class LogMessageEventArgs : EventArgs
    {
        public LogLevel Level { get; }
        public string Message { get; }
        public DateTime Timestamp { get; }
        
        public LogMessageEventArgs(LogLevel level, string message)
        {
            Level = level;
            Message = message;
            Timestamp = DateTime.Now;
        }
    }
} 