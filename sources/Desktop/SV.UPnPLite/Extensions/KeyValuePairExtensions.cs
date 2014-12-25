
namespace SV.UPnPLite.Extensions
{
	using System.Collections.Generic;

	public static class KeyValuePairExtensions
	{
		public static KeyValuePair<string, object> AsKeyFor(this string key, object value)
		{
			return new KeyValuePair<string, object>(key, value);
		}
	}
}
