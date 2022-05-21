using Microsoft.Extensions.Logging;

namespace Shared.Extensions
{
    /// <summary>
    /// Type provides useful ILogger extension methods
    /// </summary>
    public static class ILoggerExtensions
    {
        /// <summary>
        /// Formats and writes an exception log message
        /// </summary>
        /// <param name="logger">ILogger instance</param>
        /// <param name="ex">Exception to log</param>
        public static void LogException(this ILogger logger, Exception ex)
        {
            logger.LogCritical($"{ex.GetType().FullName} - {ex.Message}{Environment.NewLine}{ex.StackTrace}");
            if (ex.InnerException != null)
            {
                logger.LogCritical("Inner Exception:");
                logger.LogException(ex.InnerException);
            }
        }

        /// <summary>
        /// Begins a logical operation scope.
        /// </summary>
        /// <param name="logger">ILogger instance</param>
        /// <param name="scope">The identifier for the scope</param>
        /// <param name="value">The identifier value for the scope</param>
        /// <returns>An System.IDisposable that ends the logical operation scope on dispose</returns>
        public static IDisposable Scope(this ILogger logger, String scope, Object value)
        {
            return logger.BeginScope(new Dictionary<String, Object> { { scope, value } });
        }
    }
}
