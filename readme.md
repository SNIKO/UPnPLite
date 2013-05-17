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

* Displaying all UPnP devices found so far:

```csharp
var devicesDiscovery = new CommonUPnPDevicesDiscovery();
foreach (var device in devicesDiscovery.DiscoveredDevices)
{
  Console.WriteLine(device.FriendlyName);
}
```

* Receiving a notification when new UPnP device is added to a network:

```csharp
var devicesDiscovery = new CommonUPnPDevicesDiscovery();
devicesDiscovery.DevicesActivity.Where(e => e.Activity == DeviceActivity.Available).Subscribe(activityInfo =>
{
	Console.WriteLine("{0} found", activityInfo.Device.FriendlyName);
});
```

* Receiving a notification when UPnP device is removed from a network:

```csharp
var devicesDiscovery = new CommonUPnPDevicesDiscovery();
devicesDiscovery.DevicesActivity.Where(e => e.Activity == DeviceActivity.Gone).Subscribe(activityInfo =>
{
	Console.WriteLine("{0} gone", activityInfo.Device.FriendlyName);
});
```

* Receiving a notification when device of specific type is added to a network:

```csharp
var devicesDiscovery = new CommonUPnPDevicesDiscovery();

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
