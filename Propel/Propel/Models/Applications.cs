using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Propel.Models
{
    public class Applications
    {
        // public Dictionary<string, string> InstalledApps { get; private set; }
        public ObservableCollection<string> InstalledApps { get; private set; }

        public Applications()
        {
            InstalledApps = GetInstalledApps();
        }
        
        public static Applications Instance { get; private set; }

        static Applications()
        {
            Instance = new Applications();
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

        private class AppInfo
        {
            public string Name { get; set; }
            public string Path { get; set; }
        }
    }
}