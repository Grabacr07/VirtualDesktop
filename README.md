# VirtualDesktop

VirtualDesktop is C# wrapper for [IVirtualDesktopManager](https://msdn.microsoft.com/en-us/library/windows/desktop/mt186440%28v%3Dvs.85%29.aspx) on Windows 11 (and Windows 10).

[![Build](https://github.com/Grabacr07/VirtualDesktop/actions/workflows/build.yml/badge.svg)](https://github.com/Grabacr07/VirtualDesktop/actions/workflows/build.yml)
[![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/VirtualDesktop)](https://www.nuget.org/packages/VirtualDesktop/)
[![License](https://img.shields.io/github/license/Grabacr07/VirtualDesktop)](LICENSE)


## Features

* Switch, add, and remove a virtual desktop.
* Move the window in the same process to any virtual desktop.
* Move the window of another process to any virtual desktop (Support in version 2.0 or later).
* Pin any window or application; will be display on all desktops.
* Notification for switching, deletion, renaming, etc.
* Change the wallpaper for each desktop.


### Sample app

![](https://user-images.githubusercontent.com/1779073/152605684-2d872356-1882-4bfd-821d-d4211ccac069.gif)
[samples/VirtualDesktop.Showcase](samples/VirtualDesktop.Showcase)


## Requirements

```xml
<TargetFramework>net5.0-windows10.0.19041.0</TargetFramework>
```
* .NET 5 or 6
* Windows 10 build 19041 (20H1) or later


## Installation

Install NuGet package(s).

```powershell
PM> Install-Package VirtualDesktop
```

* [VirtualDesktop](https://www.nuget.org/packages/VirtualDesktop/) - Core classes for VirtualDesktop.
* [VirtualDesktop.WPF](https://www.nuget.org/packages/VirtualDesktop.WPF/) - Provides extension methods for WPF [Window class](https://msdn.microsoft.com/en-us/library/system.windows.window(v=vs.110).aspx).
* [VirtualDesktop.WinForms](https://www.nuget.org/packages/VirtualDesktop.WinForms/) - Provides extension methods for [Form class](https://msdn.microsoft.com/en-us/library/system.windows.forms.form(v=vs.110).aspx).


## How to use

### Preparation
Because of the dependency on [C#/WinRT](https://aka.ms/cswinrt) ([repo](https://github.com/microsoft/CsWinRT)), the target framework must be set to `net5.0-windows10.0.19041.0` or later.
```xml
<TargetFramework>net5.0-windows10.0.19041.0</TargetFramework>
```
```xml
<TargetFramework>net6.0-windows10.0.19041.0</TargetFramework>
```

If it doesn't work, try creating an `app.manifest` file and optimize to work on Windows 10.
```xml
<compatibility xmlns="urn:schemas-microsoft-com:compatibility.v1">
    <application>
	    <!-- Windows 10 / 11-->
	    <supportedOS Id="{8e0f7a12-bfb3-4fe8-b9a5-48fd50a15a9a}" />
    </application>
</compatibility>
```

The namespace to use is `WindowsDesktop`.
```csharp
using WindowsDesktop;
```

### Get instance of VirtualDesktop class
```csharp 
// Get all virtual desktops
var desktops = VirtualDesktop.GetDesktops();

// Get Virtual Desktop for specific window
var desktop = VirtualDesktop.FromHwnd(hwnd);

// Get the left/right desktop
var left  = desktop.GetLeft();
var right = desktop.GetRight();
```

### Manage virtual desktops
```csharp
// Create new
var desktop = VirtualDesktop.Create();

// Remove
desktop.Remove();

// Switch
desktop.GetLeft().Switch();
```

### Subscribe virtual desktop events
```csharp
// Notification of desktop switching
VirtualDesktop.CurrentChanged += (_, args) => Console.WriteLine($"Switched: {args.NewDesktop.Name}");

// Notification of desktop creating
VirtualDesktop.Created += (_, desktop) => desktop.Switch();
```

### for WPF window
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

### See also:
* [samples/README.md](samples/README.md)


## License

This library is under [the MIT License](https://github.com/Grabacr07/VirtualDesktop/blob/master/LICENSE).
