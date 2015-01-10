
namespace SV.UPnPLite.Protocols.UPnP
{
	using SV.UPnPLite.Extensions;

	/// <summary>
	///     Defines a version of the UPnP object (device/service).
	/// </summary>
	public struct UPnPVersion
	{
		/// <summary>
		///     The major number of the version.
		/// </summary>
		public int Major;

		/// <summary>
		///     The minor number of the version.
		/// </summary>
		public int Minor;

		/// <summary>
		///     Returns a <see cref="System.String" /> that represents this instance.
		/// </summary>
		/// <returns>
		///     A <see cref="System.String" /> that represents this instance.
		/// </returns>
		public override string ToString()
		{
			return "{0}.{1}".F(this.Major, this.Minor);
		}
	}
}
