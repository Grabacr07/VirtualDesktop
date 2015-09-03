## VirtualDesktop

VirtualDesktop is C# wrapper for [IVirtualDesktopManager](https://msdn.microsoft.com/en-us/library/windows/desktop/mt186440%28v%3Dvs.85%29.aspx) on Windows 10.


## Features

* Switch, add, and remove a Virtual Desktop.
* Move the window in the same process to any Virtual Desktop.
* **[CANNOT]** Move the window of another process to any Virtual Desktop.


## How to use

Preparation: 
```xml
<!-- create "app.manifest" file -->
<compatibility xmlns="urn:schemas-microsoft-com:compatibility.v1">
    <application>
	    <!-- Windows 10 -->
	    <supportedOS Id="{8e0f7a12-bfb3-4fe8-b9a5-48fd50a15a9a}" />
    </application>
</compatibility>
```
```csharp
// reference VirtualDesktop.dll
using WindowsDesktop;
```

Get instance of VirtualDesktop: 
```csharp 
// Get all Virtual Desktops
var desktops = VirtualDesktop.GetDesktops();

// Get Virtual Desktop for specific window
var desktop = VirtualDesktop.FromHwnd(hwnd);

// Get the left/right desktop
var left  = desktop.GetLeft();
var right = desktop.GetRight();
```

Manage Virtual Desktops:
```csharp
// Create new
var desktop = VirtualDesktop.Create();

// Remove
desktop.Remove();

// Switch
desktop.Switch();

```

for WPF window
```csharp
// reference VirtualDesktop.WPF.dll

// Check whether a window is on the current desktop.
var isCurrent = window.IsCurrentVirtualDesktop();

// Get Virtual Desktop for WPF window
var desktop = window.GetCurrentDesktop();

// Move window to specific Virtual Desktop
window.MoveToDesktop(desktop);
```

See also:
* [samples/VirtualDesktop.Showcase](samples/VirtualDesktop.Showcase) project  
![ss150904031950kd](https://cloud.githubusercontent.com/assets/1779073/9666915/d57850d8-52b3-11e5-9d61-b13a49656b11.png)


## License

This library is under the MIT License.
