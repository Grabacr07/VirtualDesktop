# VirtualDesktop

VirtualDesktop is C# wrapper for [IVirtualDesktopManager](https://msdn.microsoft.com/en-us/library/windows/desktop/mt186440%28v%3Dvs.85%29.aspx) on Windows 10 / 11.


## Features

* Switch, add, and remove a virtual desktop.
* Move the window in the same process to any virtual desktop.
* Move the window of another process to any virtual desktop (Support in version 2.0 or later).
* Pin any window or application; will be display on all desktops.
* Notification for switching, deletion, renaming, etc.


## Installation

Install NuGet package(s).

```powershell
PM> Install-Package VirtualDesktop
```

* [VirtualDesktop](https://www.nuget.org/packages/VirtualDesktop/) - Core classes for VirtualDesktop.
* [VirtualDesktop.WPF](https://www.nuget.org/packages/VirtualDesktop.WPF/) - Provides extension methods for WPF [Window class](https://msdn.microsoft.com/en-us/library/system.windows.window(v=vs.110).aspx).
* [VirtualDesktop.WinForms](https://www.nuget.org/packages/VirtualDesktop.WinForms/) - Provides extension methods for [Form class](https://msdn.microsoft.com/en-us/library/system.windows.forms.form(v=vs.110).aspx).


## How to use

Preparation: 
```xml
<!-- Please create application manifest file and run without debugging. -->
<compatibility xmlns="urn:schemas-microsoft-com:compatibility.v1">
    <application>
	    <!-- Windows 10 / 11-->
	    <supportedOS Id="{8e0f7a12-bfb3-4fe8-b9a5-48fd50a15a9a}" />
    </application>
</compatibility>
```
```csharp
using WindowsDesktop;
```

Get instance of VirtualDesktop class: 
```csharp 
// Get all virtual desktops
var desktops = VirtualDesktop.GetDesktops();

// Get Virtual Desktop for specific window
var desktop = VirtualDesktop.FromHwnd(hwnd);

// Get the left/right desktop
var left  = desktop.GetLeft();
var right = desktop.GetRight();
```

Manage virtual desktops:
```csharp
// Create new
var desktop = VirtualDesktop.Create();

// Remove
desktop.Remove();

// Switch
desktop.GetLeft().Switch();
```

Virtual desktop events:
```csharp
// Notification of desktop switching
VirtualDesktop.CurrentChanged += (_, args) => Console.WriteLine($"Switched: {args.NewDesktop.Name}");

// Notification of desktop creating
VirtualDesktop.Created += (_, desktop) => desktop.Switch();
```

for WPF window
```csharp
// Need to install 'VirtualDesktop.WPF' package

// Check whether a window is on the current desktop.
var isCurrent = window.IsCurrentVirtualDesktop();

// Get Virtual Desktop for WPF window
var desktop = window.GetCurrentDesktop();

// Move window to specific Virtual Desktop
window.MoveToDesktop(desktop);

// Pin window
window.Pin()
```

See also:
* [samples/VirtualDesktop.Showcase](samples/VirtualDesktop.Showcase) project  
![](https://user-images.githubusercontent.com/1779073/152408982-149d483f-ee5b-48da-974f-6eb0f332364d.png)

## License

This library is under [the MIT License](https://github.com/Grabacr07/VirtualDesktop/blob/master/LICENSE).
