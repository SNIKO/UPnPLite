
namespace SV.UPnPLite.Logging
{
    using System;
	using System.Collections.Generic;

    /// <summary>
    ///     Defines methods for logging debug information.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        ///     Writes a very detailed logs, which may include high-volume information such as protocol payloads. This log level is typically only enabled during development.
        /// </summary>
        /// <param name="message">
        ///     The formatted text of the log.
        /// </param>
        /// <param name="args">
        ///     Am argumets for formatted <paramref name="message"/>.
        /// </param>
        void Trace(string message, params object[] args);

        /// <summary>
        ///     Writes a very detailed logs, which may include high-volume information such as protocol payloads. This log level is typically only enabled during development.
        /// </summary>
        /// <param name="ex">
        ///     An exception associated with log message.
        /// </param>
        /// <param name="message">
        ///     The formatted text of the log.
        /// </param>
        /// <param name="args">
        ///     Am argumets for formatted <paramref name="message"/>.
        /// </param>
        void Trace(Exception ex, string message, params object[] args);

        /// <summary>
        ///     Writes a debugging information, less detailed than trace, typically not enabled in production environment.
        /// </summary>
        /// <param name="message">
        ///     The formatted text of the log.
        /// </param>
        /// <param name="args">
        ///     Am argumets for formatted <paramref name="message"/>.
        /// </param>
        void Debug(string message, params object[] args);

        /// <summary>
        ///     Writes a debugging information, less detailed than trace, typically not enabled in production environment.
        /// </summary>
        /// <param name="ex">
        ///     An exception associated with log message.
        /// </param>
        /// <param name="message">
        ///     The formatted text of the log.
        /// </param>
        /// <param name="args">
        ///     Am argumets for formatted <paramref name="message"/>.
        /// </param>
        void Debug(Exception ex, string message, params object[] args);

        /// <summary>
        ///     Writes an information messages, which are normally enabled in production environment.
        /// </summary>
        /// <param name="message">
        ///     The formatted text of the log.
        /// </param>
        /// <param name="args">
        ///     Am argumets for formatted <paramref name="message"/>.
        /// </param>
        void Info(string message, params object[] args);

        /// <summary>
        ///     Writes an information messages, which are normally enabled in production environment.
        /// </summary>
        /// <param name="ex">
        ///     An exception associated with log message.
        /// </param>
        /// <param name="message">
        ///     The formatted text of the log.
        /// </param>
        /// <param name="args">
        ///     Am argumets for formatted <paramref name="message"/>.
        /// </param>
        void Info(Exception ex, string message, params object[] args);

        /// <summary>
        ///     Writes a warning messages, typically for non-critical issues, which can be recovered or which are temporary failures.
        /// </summary>
        /// <param name="message">
        ///     The formatted text of the log.
        /// </param>
        /// <param name="args">
        ///     Am argumets for formatted <paramref name="message"/>.
        /// </param>
        void Warning(string message, params object[] args);

        /// <summary>
        ///     Writes a warning messages, typically for non-critical issues, which can be recovered or which are temporary failures.
        /// </summary>
        /// <param name="ex">
        ///     An exception associated with log message.
        /// </param>
        /// <param name="message">
        ///     The formatted text of the log.
        /// </param>
        /// <param name="args">
        ///     Am argumets for formatted <paramref name="message"/>.
        /// </param>
        void Warning(Exception ex, string message, params object[] args);

		/// <summary>
		///     Writes a warning messages, typically for non-critical issues, which can be recovered or which are temporary failures.
		/// </summary>
		/// <param name="ex">
		///     An exception associated with log message.
		/// </param>
		/// <param name="message">
		///     The formatted text of the log.
		/// </param>
		/// <param name="parameters">
		///     The list of custom parameters.
		/// </param>
		void Warning(Exception ex, string message, params KeyValuePair<string, string>[] parameters);

        /// <summary>
        ///     Writes an error message.
        /// </summary>
        /// <param name="message">
        ///     The formatted text of the log.
        /// </param>
        /// <param name="args">
        ///     Am argumets for formatted <paramref name="message"/>.
        /// </param>
        void Error(string message, params object[] args);

        /// <summary>
        ///     Writes an error message.
        /// </summary>
        /// <param name="ex">
        ///     An exception associated with log message.
        /// </param>
        /// <param name="message">
        ///     The formatted text of the log.
        /// </param>
        /// <param name="args">
        ///     Am argumets for formatted <paramref name="message"/>.
        /// </param>
        void Error(Exception ex, string message, params object[] args);

        /// <summary>
        ///     Writes a very serious error message.
        /// </summary>
        /// <param name="message">
        ///     The formatted text of the log.
        /// </param>
        /// <param name="args">
        ///     Am argumets for formatted <paramref name="message"/>.
        /// </param>
        void Critical(string message, params object[] args);

        /// <summary>
        ///     Writes a very serious error message.
        /// </summary>
        /// <param name="ex">
        ///     An exception associated with log message.
        /// </param>
        /// <param name="message">
        ///     The formatted text of the log.
        /// </param>
        /// <param name="args">
        ///     Am argumets for formatted <paramref name="message"/>.
        /// </param>
        void Critical(Exception ex, string message, params object[] args);
    }
}
