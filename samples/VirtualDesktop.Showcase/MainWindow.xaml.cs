using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using WindowsDesktop;
using WindowsDesktop.Properties;

namespace VirtualDesktopShowcase;

partial class MainWindow
{
    private const int _delay = 2000;
    private IDisposable? _applicationViewChangedListener;

    public ObservableCollection<VirtualDesktopViewModel> Desktops { get; } = new();

    public MainWindow()
    {
        this.InitializeComponent();
        this.InitializeComObjects();
    }

    private void InitializeComObjects()
    {
        VirtualDesktop.Configure();

        VirtualDesktop.Created += (_, desktop) =>
        {
            this.Dispatcher.Invoke(() =>
            {
                this.Desktops.Add(new VirtualDesktopViewModel(desktop));
                Debug.WriteLine($"Created: {desktop.Name}");
            });
        };

        VirtualDesktop.CurrentChanged += (_, args) =>
        {
            foreach (var desktop in this.Desktops) desktop.IsCurrent = desktop.Id == args.NewDesktop.Id;
            Debug.WriteLine($"Switched: {args.OldDesktop.Name} -> {args.NewDesktop.Name}");
        };

        VirtualDesktop.Moved += (_, args) =>
        {
            this.Dispatcher.Invoke(() =>
            {
                this.Desktops.Move(args.OldIndex, args.NewIndex);
                Debug.WriteLine($"Moved: {args.OldIndex} -> {args.NewIndex}, {args.Desktop}");
            });
        };

        VirtualDesktop.Destroyed += (_, args) =>
        {
            this.Dispatcher.Invoke(() =>
            {
                var target = this.Desktops.FirstOrDefault(x => x.Id == args.Destroyed.Id);
                if (target != null) this.Desktops.Remove(target);
            });
        };

        VirtualDesktop.Renamed += (_, args) =>
        {
            var desktop = this.Desktops.FirstOrDefault(x => x.Id == args.Desktop.Id);
            if (desktop != null) desktop.Name = args.Name;
            Debug.WriteLine($"Renamed: {args.Desktop}");
        };

        VirtualDesktop.WallpaperChanged += (_, args) =>
        {
            var desktop = this.Desktops.FirstOrDefault(x => x.Id == args.Desktop.Id);
            if (desktop != null) desktop.WallpaperPath = new Uri(args.Path);
            Debug.WriteLine($"Wallpaper changed: {args.Desktop}, {args.Path}");
        };

        var currentId = VirtualDesktop.Current.Id;

        foreach (var desktop in VirtualDesktop.GetDesktops())
        {
            var vm = new VirtualDesktopViewModel(desktop);
            if (desktop.Id == currentId) vm.IsCurrent = true;

            this.Desktops.Add(vm);
        }
    }

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);

        this._applicationViewChangedListener = VirtualDesktop.RegisterViewChanged(this.GetHandle(), handle =>
        {
            this.Dispatcher.Invoke(() =>
            {
                var parent = VirtualDesktop.FromHwnd(handle);
                foreach (var desktop in this.Desktops)
                {
                    desktop.ShowcaseMessage = parent == null
                        ? "(this window is pinned)"
                        : desktop.Id == parent.Id
                            ? "this window is here."
                            : "";
                }
            });
        });
    }

    protected override void OnClosed(EventArgs e)
    {
        this._applicationViewChangedListener?.Dispose();
        base.OnClosed(e);
    }

    private void CreateNew(object sender, RoutedEventArgs e)
        => VirtualDesktop.Create();

    private async void CreateNewAndMove(object sender, RoutedEventArgs e)
    {
        var desktop = VirtualDesktop.Create();

        if (this.ThisWindowMenu.IsChecked ?? true)
        {
            desktop.SwitchAndMove(this);
        }
        else
        {
            await Task.Delay(_delay);

            var handle = GetForegroundWindow();
            if (VirtualDesktop.IsPinnedWindow(handle) == false) VirtualDesktop.MoveToDesktop(handle, desktop);
            desktop.Switch();
        }
    }

    private void SwitchLeft(object sender, RoutedEventArgs e)
    {
        VirtualDesktop.Current.GetLeft()?.Switch();
    }

    private async void SwitchLeftAndMove(object sender, RoutedEventArgs e)
    {
        var left = VirtualDesktop.Current.GetLeft();
        if (left == null) return;

        if (this.ThisWindowMenu.IsChecked ?? true)
        {
            left.SwitchAndMove(this);
        }
        else
        {
            await Task.Delay(_delay);

            var handle = GetForegroundWindow();
            if (VirtualDesktop.IsPinnedWindow(handle) == false) VirtualDesktop.MoveToDesktop(handle, left);
            left.Switch();
        }
    }

    private void SwitchRight(object sender, RoutedEventArgs e)
    {
        VirtualDesktop.Current.GetRight()?.Switch();
    }

    private async void SwitchRightAndMove(object sender, RoutedEventArgs e)
    {
        var right = VirtualDesktop.Current.GetRight();
        if (right == null) return;

        if (this.ThisWindowMenu.IsChecked ?? true)
        {
            right.SwitchAndMove(this);
        }
        else
        {
            await Task.Delay(_delay);

            var handle = GetForegroundWindow();
            if (VirtualDesktop.IsPinnedWindow(handle) == false) VirtualDesktop.MoveToDesktop(handle, right);
            right.Switch();
        }
    }

    private async void Pin(object sender, RoutedEventArgs e)
    {
        if (this.ThisWindowMenu.IsChecked ?? true)
        {
            this.TogglePin();
        }
        else
        {
            await Task.Delay(_delay);

            var handle = GetForegroundWindow();
            (VirtualDesktop.IsPinnedWindow(handle) ? VirtualDesktop.UnpinWindow : (Func<IntPtr, bool>)VirtualDesktop.PinWindow)(handle);
        }
    }

    private async void PinApp(object sender, RoutedEventArgs e)
    {
        if (this.ThisWindowMenu.IsChecked ?? true)
        {
            Application.Current.TogglePin();
        }
        else
        {
            await Task.Delay(_delay);

            if (VirtualDesktop.TryGetAppUserModelId(GetForegroundWindow(), out var appId))
            {
                (VirtualDesktop.IsPinnedApplication(appId) ? VirtualDesktop.UnpinApplication : (Func<string, bool>)VirtualDesktop.PinApplication)(appId);
            }
        }
    }

    private void Remove(object sender, RoutedEventArgs e)
    {
        VirtualDesktop.Current.Remove();
    }

    private void SwitchDesktop(object sender, RoutedEventArgs e)
    {
        if (sender is Button { DataContext: VirtualDesktopViewModel vm })
        {
            VirtualDesktop.FromId(vm.Id)?.SwitchAndMove(this);
        }
    }

    private void ChangeWallpaper(object sender, RoutedEventArgs e)
    {
        if (sender is Button { DataContext: VirtualDesktopViewModel vm })
        {
            var dialog = new OpenFileDialog()
            {
                Title = "Select wallpaper",
                Filter = "Desktop wallpaper (*.jpg, *.png, *.bmp)|*.jpg;*.png;*.bmp",
            };

            if ((dialog.ShowDialog(this) ?? false)
                && File.Exists(dialog.FileName))
            {
                var desktop = VirtualDesktop.FromId(vm.Id);
                if (desktop != null) desktop.WallpaperPath = dialog.FileName;
            }
        }

        e.Handled = true;
    }

    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();
}

public class VirtualDesktopViewModel : INotifyPropertyChanged
{
    private string _name;
    private Uri? _wallpaperPath;
    private string _showcaseMessage;
    private bool _isCurrent;

    public event PropertyChangedEventHandler? PropertyChanged;

    public Guid Id { get; }

    public string Name
    {
        get => this._name;
        set
        {
            if (this._name != value)
            {
                this._name = value;
                this.OnPropertyChanged();
            }
        }
    }

    public Uri? WallpaperPath
    {
        get => this._wallpaperPath;
        set
        {
            if (this._wallpaperPath != value)
            {
                this._wallpaperPath = value;
                this.OnPropertyChanged();
            }
        }
    }

    public string ShowcaseMessage
    {
        get => this._showcaseMessage;
        set
        {
            if (this._showcaseMessage != value)
            {
                this._showcaseMessage = value;
                this.OnPropertyChanged();
            }
        }
    }

    public bool IsCurrent
    {
        get => this._isCurrent;
        set
        {
            if (this._isCurrent != value)
            {
                this._isCurrent = value;
                this.OnPropertyChanged();
            }
        }
    }

    public VirtualDesktopViewModel(VirtualDesktop source)
    {
        this._name = string.IsNullOrEmpty(source.Name) ? "(no name)" : source.Name;
        this._wallpaperPath = Uri.TryCreate(source.WallpaperPath, UriKind.Absolute, out var uri) ? uri : null;
        this._showcaseMessage = "";
        this.Id = source.Id;
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
