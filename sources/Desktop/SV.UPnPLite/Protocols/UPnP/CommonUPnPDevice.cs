
namespace SV.UPnPLite.Protocols.UPnP
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	///     Allows low level control of any UPnP device.
	/// </summary>
	public class CommonUPnPDevice : UPnPDevice
	{
		#region Constructors

		/// <summary>
		///     Initializes a new instance of the <see cref="CommonUPnPDevice"/> class.
		/// </summary>
		/// <param name="udn">
		///     A universally-unique identifier for the device.
		/// </param>
		/// <exception cref="ArgumentNullException">
		///     <paramref name="udn"/> is <c>nukk</c> or <see cref="string.Empty"/>.
		/// </exception>
		public CommonUPnPDevice(string udn)
			: base(udn)
		{
		}

		#endregion

		#region Properties

		/// <summary>
		///     Gets a list of UPnP services provided by the device.
		/// </summary>
		public IEnumerable<UPnPService> Services { get; internal set; }

		#endregion
	}
}
