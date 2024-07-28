using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Microsoft.Win32;
using ReactiveUI;

namespace Propel.ViewModels
{
    public class CreateLaunchViewModel : ViewModelBase
    {
        private string? _searchText;
        public string? SearchText
        {
            get => _searchText;
            set => this.RaiseAndSetIfChanged(ref _searchText, value);
        }

        private ObservableCollection<string> _installedApps;
        public ObservableCollection<string> InstalledApps
        {
            get => _installedApps;
            set => this.RaiseAndSetIfChanged(ref _installedApps, value);
        }

        private ObservableCollection<string> _filteredInstalledApps;
        public ObservableCollection<string> FilteredInstalledApps
        {
            get => _filteredInstalledApps;
            set => this.RaiseAndSetIfChanged(ref _filteredInstalledApps, value);
        }

        public CreateLaunchViewModel()
        {
            InstalledApps = GetInstalledApps();
            FilteredInstalledApps = new ObservableCollection<string>(InstalledApps);
            this.WhenAnyValue(x => x.SearchText)
                .Throttle(TimeSpan.FromMilliseconds(400))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(searchQuery => FilterInstalledApps(searchQuery));
        }

        private ObservableCollection<string> GetInstalledApps()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return GetInstalledAppsWindows();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return GetInstalledAppsMac();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return GetInstalledAppsLinux();
            }
            else
            {
                return new ObservableCollection<string>();
            }
        }

        private void FilterInstalledApps(string? searchQuery)
        {
            if (string.IsNullOrEmpty(searchQuery))
            {
                FilteredInstalledApps = new ObservableCollection<string>(InstalledApps);
            }
            else
            {
                var filtered = InstalledApps
                    .Where(app => app.Contains(searchQuery, StringComparison.OrdinalIgnoreCase))
                    .ToList();
                FilteredInstalledApps = new ObservableCollection<string>(filtered);
            }
        }

        private ObservableCollection<string> GetInstalledAppsWindows()
        {
            var apps = new ObservableCollection<string>();
            try
            {
                string registryKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(registryKey))
                {
                    if (key != null)
                    {
                        foreach (string subkeyName in key.GetSubKeyNames())
                        {
                            using (RegistryKey subkey = key.OpenSubKey(subkeyName))
                            {
                                var displayName = subkey.GetValue("DisplayName") as string;
                                if (!string.IsNullOrEmpty(displayName))
                                {
                                    apps.Add(displayName);
                                }
                            }
                        }
                    }
                }

                registryKey = @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall";
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(registryKey))
                {
                    if (key != null)
                    {
                        foreach (string subkey_name in key.GetSubKeyNames())
                        {
                            using (RegistryKey subkey = key.OpenSubKey(subkey_name))
                            {
                                var displayName = subkey.GetValue("DisplayName") as string;
                                if (!string.IsNullOrEmpty(displayName) && !apps.Contains(displayName))
                                {
                                    apps.Add(displayName);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching installed apps: {ex.Message}");
            }

            return apps;
        }

        private ObservableCollection<string> GetInstalledAppsMac()
        {
            var apps = new ObservableCollection<string>();
            //Need to implement in the future
            return apps;
        }

        private ObservableCollection<string> GetInstalledAppsLinux()
        {
            var apps = new ObservableCollection<string>();
            //Need to implement in the future
            return apps;
        }
    }
}
