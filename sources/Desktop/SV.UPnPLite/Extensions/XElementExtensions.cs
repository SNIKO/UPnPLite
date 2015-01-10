
namespace SV.UPnPLite.Extensions
{
	using System.Xml.Linq;

	/// <summary>
	///     Defines extension methods for <see cref="XElement"/>.
	/// </summary>
	public static class XElementExtensions
	{
		/// <summary>
		///     Returns a value of the <paramref name="element"/>.
		/// </summary>
		/// <param name="element">
		///     An <see cref="XElement"/> for which to return value.
		/// </param>
		/// <returns>
		///     A <see cref="XElement.Value"/> if <paramref name="element"/> not <c>null</c>; otherwise, <see cref="string.Empty"/>.
		/// </returns>
		public static string ValueOrDefault(this XElement element)
		{
			if (element != null)
			{
				return element.Value;
			}
			else
			{
				return string.Empty;
			}
		}
	}
}
