
namespace SV.UPnPLite.Protocols.DLNA.Services.ContentDirectory.Extensions
{
    using System;

    /// <summary>
    ///     Defines extension methods for <see cref="string"/>.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        ///     Checks whether the string is euqal to <see cref="compareTo"/> with ignoring case.
        /// </summary>
        /// <param name="st">
        ///     The original string.
        /// </param>
        /// <param name="compareTo">
        ///     The string compare to.
        /// </param>
        /// <returns>
        ///     <c>true</c>, if <paramref name="st"/> and <paramref name="compareTo"/> are equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool Is(this string st, string compareTo)
        {
            return StringComparer.OrdinalIgnoreCase.Compare(st, compareTo) == 0;
        }
    }
}
