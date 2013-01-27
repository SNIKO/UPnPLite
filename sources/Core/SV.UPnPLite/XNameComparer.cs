
namespace SV
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Linq;

    /// <summary>
    ///     The base class for all <see cref="XName"/> comparers.
    /// </summary>
    public abstract class XNameComparer : IEqualityComparer<XName>
    {
        #region Properties
        /// <summary>
        ///     Gets a <see cref="XNameComparer"/> object that performs a case-insensitive ordinal <see cref="XName"/> comparison.
        /// </summary>
        public static XNameComparer OrdinalIgnoreCase
        {
            get
            {
                return new OrdinalIgnoreCaseComparer();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     When overridden in a derived class, indicates whether two <see cref="XName"/> are equal.
        /// </summary>
        /// <param name="x">
        ///     A <see cref="XName"/> to compare to <paramref name="y"/>.
        /// </param>
        /// <param name="y">
        ///     A <see cref="XName"/> to compare to <paramref name="x"/>.
        /// </param>
        /// <returns>
        ///     <c>true</c> if <paramref name="x"/> and <paramref name="y"/> refer to the same object, or <paramref name="x"/> and <paramref name="y"/> are equal; otherwise, <c>false</c>.
        /// </returns>
        public abstract bool Equals(XName x, XName y);

        /// <summary>
        ///     Returns a hash code for this instance.
        /// </summary>
        /// <param name="obj">
        ///     The <see cref="XName"/>.
        /// </param>
        /// <returns>
        ///     A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public abstract int GetHashCode(XName obj);

        #endregion

        #region Concrete Comparers

        private class OrdinalIgnoreCaseComparer : XNameComparer
        {
            public override bool Equals(XName x, XName y)
            {
                return string.Compare(x.ToString(), y.ToString(), StringComparison.OrdinalIgnoreCase) == 0;
            }

            public override int GetHashCode(XName obj)
            {
                return obj.GetHashCode();
            }
        }

        #endregion
    }
}
