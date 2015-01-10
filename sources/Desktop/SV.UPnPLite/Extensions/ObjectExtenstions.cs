
namespace SV.UPnPLite.Extensions
{
	using System;

	/// <summary>
	///     Defines extension methods for <see cref="object"/>.
	/// </summary>
	public static class ObjectExtenstions
	{
		/// <summary>
		///     Throws an <see cref="ArgumentNullException"/> if <paramref name="obj"/> is <c>null</c> or <see cref="string.Empty"/>.
		/// </summary>
		/// <param name="obj">
		///     An onject to check.
		/// </param>
		/// <param name="name">
		///     The name of the argument to put into <see cref="ArgumentException.ParamName"/>.
		/// </param>
		/// <exception cref="ArgumentNullException">
		///     <paramref name="obj"/> is <c>null</c> or <see cref="string.Empty"/>.
		/// </exception>
		public static void EnsureNotNull(this object obj, string name)
		{
			if (obj == null)
			{
				throw new ArgumentNullException(name);
			}

			if (obj is string)
			{
				if (string.IsNullOrEmpty((string)obj))
				{
					throw new ArgumentNullException(name);
				}
			}
		}

		/// <summary>
		///     Returns string representation of <see cref="obj"/>.
		/// </summary>
		/// <param name="obj">
		///     The object for which to return the string representation.
		/// </param>
		/// <returns>
		///     The string representation of <paramref name="obj"/> if it is not <c>null</c>; otherwise, <see cref="string.Empty"/>.
		/// </returns>
		public static string ToStringOrEmpty(this object obj)
		{
			if (obj != null)
			{
				return obj.ToString();
			}
			else
			{
				return string.Empty;
			}
		}
	}
}
