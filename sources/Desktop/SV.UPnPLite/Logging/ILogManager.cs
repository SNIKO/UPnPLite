
namespace SV.UPnPLite.Logging
{
	using System;

	/// <summary>
	///     Defines methods for creating <see cref="ILogger"/> instances.
	/// </summary>
	public interface ILogManager
	{
		/// <summary>
		///     Returns logger by its name. 
		/// </summary>
		/// <param name="name">
		///     The name of the logger to return.
		/// </param>
		/// <returns>
		///     An instance of the concrete logger.
		/// </returns>
		ILogger GetLogger(string name);

		/// <summary>
		///     Returns logger for a specified type. 
		/// </summary>
		/// <typeparam name="T">
		///     The type for which to return logger.
		/// </typeparam>
		/// <returns>
		///     An instance of the concrete logger for a <typeparamref name="T"/>.
		/// </returns>
		ILogger GetLogger<T>();

		/// <summary>
		///     Returns logger for a specified type. 
		/// </summary>
		/// <param name="type">
		///     The type for which to return logger.
		/// </param>
		/// <returns>
		///     An instance of the concrete logger for a <paramref name="type"/>.
		/// </returns>
		ILogger GetLogger(Type type);
	}
}