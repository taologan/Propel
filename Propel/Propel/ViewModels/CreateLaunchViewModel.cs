using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using Propel.Models;
using Propel.Views;
using ReactiveUI;

namespace Propel.ViewModels
{
    public class CreateLaunchViewModel : ViewModelBase
    {
        private string _launchName;
        public string LaunchName
        {
            get => _launchName;
            set => this.RaiseAndSetIfChanged(ref _launchName, value);
        }

        public ObservableCollection<Launch> Launches { get; }

        public ReactiveCommand<Unit, Unit> OpenFileDialogCommand { get; }
        public ReactiveCommand<Unit, Unit> SaveLaunchCommand { get; }
        public ReactiveCommand<Launch, Unit> LaunchAppsCommand { get; }

        public ReactiveCommand<Unit, LaunchViewModel> CreateLaunchCommand { get; }

        private LaunchViewModel? _launch;
        public LaunchViewModel? Launch
        {
            get => _launch;
            set => this.RaiseAndSetIfChanged(ref _launch, value);
        }

        public CreateLaunchViewModel()
        {
            Launches = new ObservableCollection<Launch>();
            OpenFileDialogCommand = ReactiveCommand.CreateFromTask(OpenFileDialogAsync);
            // SaveLaunchCommand = ReactiveCommand.Create(SaveLaunch);
            LaunchAppsCommand = ReactiveCommand.Create<Launch>(LaunchApps);
            CreateLaunchCommand = ReactiveCommand.Create(() => Launch);
        }

        private async Task OpenFileDialogAsync()
        {
            var topLevel = TopLevel.GetTopLevel(Avalonia.Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop ? desktop.MainWindow : null);

            if (topLevel == null)
                return;

            var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Open Applications",
                AllowMultiple = true
            });

            if (files.Count >= 1)
            {
                if (LaunchName == null)
                {
                    Console.WriteLine("Please enter a launch name.");
                    return;
                }

                var launch = Launches.FirstOrDefault(l => l.Name == LaunchName) ?? new Launch { Name = LaunchName };

                foreach (var file in files)
                {
                    launch.FilePaths.Add(file.Path.LocalPath);
                }

                if (!Launches.Contains(launch))
                {
                    Launches.Add(launch);
                }
                var launchviewmodel = new Launch { Name = LaunchName, FilePaths = new ObservableCollection<string>(Launches.SelectMany(l => l.FilePaths)) };
                Launch = new LaunchViewModel(launchviewmodel);
            }
        }
        private void SaveLaunch()
        {
            Console.WriteLine($"Launch '{LaunchName}' saved.");
            var launch = new Launch { Name = LaunchName, FilePaths = new ObservableCollection<string>(Launches.SelectMany(l => l.FilePaths)) };
            Launch = new LaunchViewModel(launch);
            // return Launch;
        }

        private void LaunchApps(Launch launch)
        {
            foreach (var filePath in launch.FilePaths)
            {
                if (File.Exists(filePath))
                {
                    Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
                }
                else
                {
                    Console.WriteLine($"File does not exist: {filePath}");
                }
            }
        }
    }
}
