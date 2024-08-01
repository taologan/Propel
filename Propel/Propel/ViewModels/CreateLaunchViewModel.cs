﻿using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Microsoft.Win32;
using Propel.Models;
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
            // InstalledApps = GetInstalledApps();
            InstalledApps = new ObservableCollection<string>(Applications.Instance.InstalledApps);
            FilteredInstalledApps = new ObservableCollection<string>(InstalledApps);
            this.WhenAnyValue(x => x.SearchText)
                .Throttle(TimeSpan.FromMilliseconds(400))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(searchQuery => FilterInstalledApps(searchQuery));
        }

        [DllImport("msi.dll", CharSet = CharSet.Auto)]
        private static extern uint MsiEnumProducts(int iProductIndex, StringBuilder lpProductBuf);

        [DllImport("msi.dll", CharSet = CharSet.Auto)]
        private static extern uint MsiGetProductInfo(string szProduct, string szAttribute, StringBuilder lpValueBuf, ref uint pcchValueBuf);

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
                using (var process = new Process())
                {
                    process.StartInfo.FileName = "powershell.exe";
                    process.StartInfo.Arguments = "-Command \"Get-Package | Select-Object -ExpandProperty Name\"";
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.CreateNoWindow = true;

                    process.Start();

                    string output = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();

                    var lines = output.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var line in lines)
                    {
                        apps.Add(line);
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
