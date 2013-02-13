
namespace SV.UPnPLite.Extensions
{
    using SV.UPnPLite.Logging;
    using System;

    /// <summary>
    ///     Defines extension methods for <see cref="ILogger"/>.
    /// </summary>
    public static class LoggerExtensions
    {
        /// <summary>
        ///     Ensures that instance exists. If it doesn't - the stub is returned.
        /// </summary>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static ILogger Instance(this ILogger logger)
        {
            if (logger != null)
            {
                return logger;
            }
            else
            {
                return new StubLogger();
            }
        }

        internal class StubLogger : ILogger
        {
            public void Trace(string message, params object[] args)
            {
            }

            public void Trace(Exception ex, string message, params object[] args)
            {
            }

            public void Debug(string message, params object[] args)
            {
            }

            public void Debug(Exception ex, string message, params object[] args)
            {
            }

            public void Info(string message, params object[] args)
            {
            }

            public void Info(Exception ex, string message, params object[] args)
            {
            }

            public void Warning(string message, params object[] args)
            {
            }

            public void Warning(Exception ex, string message, params object[] args)
            {
            }

            public void Error(string message, params object[] args)
            {
            }

            public void Error(Exception ex, string message, params object[] args)
            {
            }

            public void Critical(string message, params object[] args)
            {
            }

            public void Critical(Exception ex, string message, params object[] args)
            {
            }
        }
    }
}
