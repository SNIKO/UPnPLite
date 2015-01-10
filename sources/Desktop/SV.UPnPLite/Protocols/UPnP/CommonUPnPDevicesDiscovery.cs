
namespace SV.UPnPLite.Protocols.UPnP
{
	using SV.UPnPLite.Logging;
	using System;
	using System.Collections.Generic;

	/// <summary>
	///     Discovers all UPnP devices over the network.
	/// </summary>
	public class CommonUPnPDevicesDiscovery : UPnPDevicesDiscovery<CommonUPnPDevice>
	{
		#region Constructors

		/// <summary>
		///     Initializes a new instance of the <see cref="CommonUPnPDevicesDiscovery" /> class.
		/// </summary>
		public CommonUPnPDevicesDiscovery()
			: base("upnp:rootdevice")
		{
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="CommonUPnPDevicesDiscovery" /> class.
		/// </summary>
		/// <param name="logManager">
		///     The <see cref="ILogManager"/> to use for logging the debug information.
		/// </param>
		/// <exception cref="ArgumentNullException">
		///     <paramref name="logManager"/> is <c>null</c>.
		/// </exception>
		public CommonUPnPDevicesDiscovery(ILogManager logManager)
			: base("upnp:rootdevice", logManager)
		{
		}

		#endregion

		#region Methods

		/// <summary>
		///     Creates an instance of concrete <see cref="UPnPDevice"/> which manages a device.
		/// </summary>
		/// <param name="udn">
		///     A universally-unique identifier for the device.
		/// </param>
		/// <param name="name">
		///     A friendly name of the device.
		/// </param>
		/// <param name="services">
		///     A set of UPnP service found on the device.
		/// </param>
		/// <returns>
		///     A concrete instance of the <see cref="UPnPDevice"/> if all reuqired service available; otherwise, <c>null</c>.
		/// </returns>
		protected override CommonUPnPDevice CreateDeviceInstance(string udn, string name, IEnumerable<UPnPService> services)
		{
			var device = new CommonUPnPDevice(udn) { Services = services };

			return device;
		}

		/// <summary>
		///     Creates an instance of concrere <see cref="UPnPService"/> which manages concrete service on a device.
		/// </summary>
		/// <param name="serviceType">
		///     A type of the service.
		/// </param>
		/// <param name="controlUri">
		///     An URL for sending commands to the service.
		/// </param>
		/// <param name="eventsUri">
		///     An URL for subscrinbing to service's events.
		/// </param>
		/// <returns>
		///     A concrete instance of the <see cref="UPnPService"/>.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		///     <paramref name="serviceType"/> is <c>null</c> or <see cref="string.Empty"/> -OR-
		///     <paramref name="controlUri"/> is <c>null</c> -OR-
		///     <paramref name="eventsUri"/> is <c>null</c>.
		/// </exception>
		protected override UPnPService CreateServiceInstance(string serviceType, Uri controlUri, Uri eventsUri)
		{
			return new UPnPService(serviceType, controlUri, eventsUri, this.logManager);
		}

		#endregion
	}
}
