
namespace SV.UPnPLite.Extensions
{
	using System.Collections.Generic;

	public static class KeyValuePairExtensions
	{
		public static KeyValuePair<string, string> AsKeyFor(this string key, string value)
		{
			return new KeyValuePair<string, string>(key, value);
		}
	}
}
