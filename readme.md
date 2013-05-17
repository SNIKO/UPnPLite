## UPnPLite

The UPnPLite is a high-performance, lite UPnP/DLNA framework for discovering and controlling media devices over the network. 

### Supported Platforms

Currently, the next platforms are supported:
* .NET 4.5
* WinRT
* Windows Phone 8

### Prerequisites:

In order to build sources, be sure that next prerequisites are installed:

* .NET Framework 4.5
* Windows Phone 8 SDK
* Reactive Extensions v2.0.20823 and upper

### Installation

The installation process is quite manual at the moment, but it Will be improved subsequently.

To start using the UPnPLite, please follow next steps:

* Download sources 
<code>git clone https://github.com/SNIKO/UPnPLite.git</code>

* Build <code>\sources\UPnPLite.sln</code> solution;

* Run <code>RegisterBinaries.bat</code> script. It will register binaries into GAC to make them available in "Reference Manager";

* Add <code>UPnPLite.dll</code> assembly to your project and enjoy using it.

### Usage

#### UPnP Examples

* Discovering UPnP devices:

```csharp
// Receiving a notification when new UPnP device is added to a network:
var devicesDiscovery = new CommonUPnPDevicesDiscovery();

// Enumerating currently available UPnP devices
var devicesDiscovery = new CommonUPnPDevicesDiscovery();
foreach (var device in devicesDiscovery.DiscoveredDevices)
{
  Console.WriteLine(device.FriendlyName);
}

devicesDiscovery.DevicesActivity.Where(e => e.Activity == DeviceActivity.Available).Subscribe(activityInfo =>
{
	Console.WriteLine("{0} found", activityInfo.Device.FriendlyName);
});

// Receiving a notification when UPnP device is removed from a network
devicesDiscovery.DevicesActivity.Where(e => e.Activity == DeviceActivity.Gone).Subscribe(activityInfo =>
{
	Console.WriteLine("{0} gone", activityInfo.Device.FriendlyName);
});

// Receiving a notification when device of specific type is added to a network
var newMediaServers = from activityInfo in devicesDiscovery.DevicesActivity
		  where activityInfo.Activity == DeviceActivity.Available && activityInfo.Device.DeviceType == "urn:schemas-upnp-org:device:MediaServer"
		  select activityInfo.Device;

newMediaServers.Subscribe(server => 
{
	Console.WriteLine("{0} found", server.FriendlyName);
});
```

* Invoking action on UPnP service:

```csharp
var devicesDiscovery = new CommonUPnPDevicesDiscovery();
var rendererDevice = devicesDiscovery.DiscoveredDevices.FirstOrDefault(d => d.DeviceType == "urn:schemas-upnp-org:device:MediaRenderer");
var renderingControlService = rendererDevice.Services.FirstOrDefault(service => service.ServiceType == "urn:upnp-org:serviceId:RenderingControl");
			
var args = new Dictionary<string, object>();
args["InstanceID"] = 0;
args["Channel"] = "Master";
args["DesiredVolume"] = 80;

var response = await renderingControlService.InvokeActionAsync("SetVolume", args);
```

#### DLNA Examples

* Discovering media devices:

```csharp
var mediaServersDiscovery = new MediaServersDiscovery();
var mediaRenderersDiscovery = new MediaRenderersDiscovery();

// Enumerating currently available servers
foreach (var server in mediaServersDiscovery.DiscoveredDevices)
{
	Console.WriteLine("Server found: {0}", server.FriendlyName);
}

// Receiving notifications when a new media server is added to a network
mediaServersDiscovery.DevicesActivity.Where(e => e.Activity == DeviceActivity.Available).Subscribe(e =>
{
	Console.WriteLine("Server found: {0}", e.Device.FriendlyName);
});

// Receiving notifications when the media renderer is removed from the network
mediaRenderersDiscovery.DevicesActivity.Where(e => e.Activity == DeviceActivity.Gone).Subscribe(e =>
{
	Console.WriteLine("Renderer gone: {0}", e.Device.FriendlyName);
});
```

* Browsing the MediaServer:

```csharp
var mediaServersDiscovery = new MediaServersDiscovery();
var server = mediaServersDiscovery.DiscoveredDevices.First();

// Requesting root media objects
var rootObjects = await server.BrowseAsync();
var rootContainers = rootObjects.OfType<MediaContainer>();
var rootMediaItems = rootObjects.OfType<MediaItem>();			

// Requesting media objects from child container
var containerToBrowse = rootContainers.First();
var childContainerObjects = await server.BrowseAsync(containerToBrowse);
```

* Searching for media items on MediaServer:

```csharp
var mediaServersDiscovery = new MediaServersDiscovery();
var server = mediaServersDiscovery.DiscoveredDevices.First();

// Requesting all music tracks
var musicTracks = await server.SearchAsync<MusicTrack>();
foreach (var track in musicTracks)
{
	Console.WriteLine("Title={0}, Album={1}, Artist={2}", track.Title, track.Album, track.Artist);
}

// Requesting all video items
var videos = await server.SearchAsync<VideoItem>();
foreach (var video in videos)
{
	Console.WriteLine("Title={0}, Genre={1}", video.Title, video.Genre);
}
```

* Playback controlling on MediaRenderer:

```csharp
var mediaServersDiscovery = new MediaServersDiscovery();
var mediaRenderersDiscovery = new MediaRenderersDiscovery();
			
var server = mediaServersDiscovery.DiscoveredDevices.First();
var renderer = mediaRenderersDiscovery.DiscoveredDevices.First();

var musicTracks = await server.SearchAsync<MusicTrack>();
var trackToPlay = musicTracks.First();
						
await renderer.OpenAsync(trackToPlay);
await renderer.PlayAsync();

// Subscribing for a playback position change
renderer.PositionChanges.Subscribe(position =>
{
	Console.WriteLine("Position changed: {0}", position);
});

// Subscribing for a playback state change
renderer.StateChanges.Subscribe(state =>
{
	Console.WriteLine("Playback state changed: {0}", state);
});
```

## Copyright

Copyright (c) 2013 Sergii Vashchyshchuk
