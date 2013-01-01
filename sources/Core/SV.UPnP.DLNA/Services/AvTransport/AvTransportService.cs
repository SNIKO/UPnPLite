
namespace SV.UPnP.DLNA.Services.AvTransport
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    ///     Enables control over the transport of audio and video streams. The service type defines a  ‘common’ model for A/V transport control suitable for a 
    ///     generic user interface. It can be used to control a wide variety of disc, tape and solid-state based media devices such as CD players, VCRs and MP3 players. 
    /// </summary>
    public class AvTransportService : ServiceBase
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instanceId of the <see cref="AvTransportService" /> class.
        /// </summary>
        /// <param name="serviceInfo">
        ///     Defines parameters of the service.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="serviceInfo"/> is <c>null</c>.
        /// </exception>
        public AvTransportService(ServiceInfo serviceInfo)
            : base(serviceInfo)
        {

        }

        #endregion

        #region Methods

        /// <summary>
        ///      Specifies the URI of the resource to be controlled by the specified AVTransport instance.
        /// </summary>
        /// <param name="instanceId">
        ///      Identifies the virtual instanceId of the AVTransport service to which the action applies.
        /// </param>
        /// <param name="currentUri">
        ///     The URI to the resource to control.
        /// </param>
        /// <param name="currentUriMetadata">
        ///     The metadata, in the form of a DIDL-Lite XML fragment, associated with the resource pointed by <paramref name="currentUri"/>.
        /// </param>
        /// <returns>
        ///     A <see cref="Task"/> instance which could be use for waiting an operation to complete.
        /// </returns>
        /// <exception cref="DeviceException">
        ///     An error occurred when sending request to device -OR-
        ///     An error occurred when executing request on device -OR-
        ///     The specified <paramref name="instanceId"/> is invalid -OR-
        ///     The DNS Server is not available -OR-
        ///     Unable to resolve the Fully Qualified Domain Name -OR-
        ///     The server that hosts the resource is unreachable or unresponsive -OR-
        ///     The specified resource cannot be found in the network -OR-
        ///     The resource is already being played by other means -OR-
        ///     The specified resource has a MIME-type which is not supported by the AVTransport service.
        /// </exception>
        public async Task SetAvTransportURIAsync(uint instanceId, Uri currentUri, string currentUriMetadata)
        {
            var arguments = new Dictionary<string, object>
                                    {
                                        { "InstanceID", instanceId },
                                        { "CurrentURI", currentUri.ToString() },
                                        { "CurrentURIMetaData", currentUriMetadata },
                                    };

            await this.InvokeActionAsync("SetAVTransportURI", arguments);
        }

        /// <summary>
        ///     Starts playing the resource of the specified instanceId, at the specified speed, starting at the current position, according to the current play mode.
        /// </summary>
        /// <param name="instanceId">
        ///      Identifies the virtual instanceId of the AVTransport service to which the action applies.
        /// </param>
        /// <param name="speed">
        ///     Indicates the speed relative to normal speed. Example values are ‘1’, ‘1/2’, ‘2’, ‘-1’, ‘1/10’, etc.
        /// </param>
        /// <returns>
        ///     A <see cref="Task"/> instance which could be use for waiting an operation to complete.
        /// </returns>
        /// <exception cref="DeviceException">
        ///     An error occurred when sending request to device -OR-
        ///     An error occurred when executing request on device -OR-
        ///     The specified <paramref name="instanceId"/> is invalid -OR-
        ///     The specified <paramref name="speed"/> is not supported -OR-
        ///     The resource to be played cannot be found in the network -OR-
        ///     The resource is already being played by other means.  The actual implementation might detect through HTTP Busy, and returns this error code. -OR-
        ///     The resource to be played has a MIME-type which is not supported by the AVTransport service -OR-
        ///     The transport is “hold locked” -OR-
        ///     The storage format of the currently loaded media is not supported for playback by this device -OR-
        ///     The media cannot be read (e.g., because of dust or a scratch) -OR-
        ///     The media does not contain any contents that can be played -OR-
        ///     The immediate transition from current transport state to desired transport state is not supported by this device.
        /// </exception>
        public async Task PlayAsync(uint instanceId, string speed)
        {
            var arguments = new Dictionary<string, object>
                                    {
                                        { "InstanceID", instanceId },
                                        { "Speed", speed },
                                    };

            await this.InvokeActionAsync("Play", arguments);
        }

        /// <summary>
        ///      Halts the progression of the resource that is associated with the specified instance.
        /// </summary>
        /// <param name="instanceId">
        ///      Identifies the virtual instanceId of the AVTransport service to which the action applies.
        /// </param>
        /// <returns>
        ///     A <see cref="Task"/> instance which could be use for waiting an operation to complete.
        /// </returns>
        /// <exception cref="DeviceException">
        ///     An error occurred when sending request to device -OR-
        ///     An error occurred when executing request on device -OR-
        ///     The specified <paramref name="instanceId"/> is invalid -OR-
        ///     The transport is “hold locked” -OR-        
        ///     The immediate transition from current transport state to desired transport state is not supported by this device.
        /// </exception>
        public async Task PauseAsync(uint instanceId)
        {
            await this.InvokeActionAsync("Pause", new Dictionary<string, object> { { "InstanceID", instanceId } });
        }

        /// <summary>
        ///      Stops the progression of the current resource that is associated with the specified instance.
        /// </summary>
        /// <param name="instanceId">
        ///      Identifies the virtual instanceId of the AVTransport service to which the action applies.
        /// </param>
        /// <returns>
        ///     A <see cref="Task"/> instance which could be use for waiting an operation to complete.
        /// </returns>
        /// <exception cref="DeviceException">
        ///     An error occurred when sending request to device -OR-
        ///     An error occurred when executing request on device -OR-
        ///     The specified <paramref name="instanceId"/> is invalid -OR-
        ///     The transport is “hold locked” -OR-        
        ///     The immediate transition from current transport state to desired transport state is not supported by this device.
        /// </exception>
        public async Task StopAsync(uint instanceId)
        {
            await this.InvokeActionAsync("Stop", new Dictionary<string, object> { { "InstanceID", instanceId } });
        }

        #endregion
    }
}
