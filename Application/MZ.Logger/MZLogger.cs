using System;
using System.Runtime.CompilerServices;
using Serilog;

namespace MZ.Logger
{
    public enum LogLevel
    {
        Information,
        Warning,
        Error
    }

    public class MZLogger
    {
        private static readonly Lazy<ILogger> _instance = new(new LoggerConfiguration()
                        .WriteTo.Logger(lc => lc
                            .Filter.ByIncludingOnly(evt => evt.Level == Serilog.Events.LogEventLevel.Information)
                            .Enrich.WithProperty("Project", "MZ.App")
                            .WriteTo.Console()
                            .WriteTo.File("Logs/info-.txt", rollingInterval: RollingInterval.Hour, fileSizeLimitBytes: 100_000_000, retainedFileCountLimit: 31, shared: true))
                        .WriteTo.Logger(lc => lc
                            .Filter.ByIncludingOnly(evt => evt.Level == Serilog.Events.LogEventLevel.Warning)
                            .Enrich.WithProperty("Project", "MZ.App")
                            .WriteTo.Console()
                            .WriteTo.File("Logs/warning-.txt", rollingInterval: RollingInterval.Hour, fileSizeLimitBytes: 100_000_000, retainedFileCountLimit: 31, shared: true))
                        .WriteTo.Logger(lc => lc
                            .Filter.ByIncludingOnly(evt => evt.Level == Serilog.Events.LogEventLevel.Error)
                            .Enrich.WithProperty("Project", "MZ.App")
                            .WriteTo.Console()
                            .WriteTo.File("Logs/error-.txt", rollingInterval: RollingInterval.Hour, fileSizeLimitBytes: 100_000_000, retainedFileCountLimit: 31, shared: true))
                        .CreateLogger());
        public static ILogger Instance => _instance.Value;

        public static void Information(string message = null, [CallerMemberName] string callerName = null)
            => Log(LogLevel.Information, message, callerName);

        public static void Warning(string message = null, [CallerMemberName] string callerName = null)
            => Log(LogLevel.Warning, message, callerName);

        public static void Error(string message = null, [CallerMemberName] string callerName = null)
            => Log(LogLevel.Error, message, callerName);

        private static void Log(LogLevel level, string message, string callerName)
        {
            var formattedMessage = FormatMessage(message, callerName);

            switch (level)
            {
                case LogLevel.Information:
                    Instance.Information(formattedMessage);
                    break;
                case LogLevel.Warning:
                    Instance.Warning(formattedMessage);
                    break;
                case LogLevel.Error:
                    Instance.Error(formattedMessage);
                    break;
                default:
                    break;
            }
        }

        private static string FormatMessage(string message, string callerName)
        {
            return string.IsNullOrWhiteSpace(message)
                ? callerName
                : $"[{callerName}] {message}";
        }
    }
}
