using System;
using System.Runtime.CompilerServices;
using Serilog;

namespace MZ.Logger
{
    /// <summary>
    /// 로그 레벨을 정의
    /// </summary>
    public enum LogLevel
    {
        Information,
        Warning,
        Error
    }

    /// <summary>
    /// Serilog로거 기준 커스텀
    /// </summary>
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

        /// <summary>
        /// Serilog ILogger 싱글턴 인스턴스
        /// </summary>
        public static ILogger Instance => _instance.Value;

        public static void Information(string message = null, [CallerMemberName] string callerName = null)
            => Log(LogLevel.Information, message, callerName);

        public static void Warning(string message = null, [CallerMemberName] string callerName = null)
            => Log(LogLevel.Warning, message, callerName);

        public static void Error(string message = null, [CallerMemberName] string callerName = null)
            => Log(LogLevel.Error, message, callerName);

        /// <summary>
        /// 로그 레벨별로 메시지를 기록하는 내부 메서드
        /// </summary>
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

        /// <summary>
        /// 메시지 포맷팅: 메시지가 없으면 호출 메서드명을, 있으면 [호출메서드] 메시지 형태로 반환
        /// </summary>
        private static string FormatMessage(string message, string callerName)
        {
            return string.IsNullOrWhiteSpace(message) ? callerName : $"[{callerName}] {message}";
        }
    }
}
