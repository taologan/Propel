using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Propel.Models
{
    public class Launch : INotifyPropertyChanged
    {
        public string PathString => ToString();
        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
                OnPropertyChanged(nameof(PathString));
            }
        }

        private ObservableCollection<string> _filePaths;
        public ObservableCollection<string> FilePaths
        {
            get => _filePaths;
            set
            {
                _filePaths = value;
                OnPropertyChanged(nameof(FilePaths));
                OnPropertyChanged(nameof(PathString));
            }
        }

        public Launch()
        {
            FilePaths = new ObservableCollection<string>();
            FilePaths.CollectionChanged += (sender, args) => OnPropertyChanged(nameof(PathString));
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Launch Name: {Name}");
            sb.AppendLine("File Paths:");

            foreach (var filePath in FilePaths)
            {
                if (File.Exists(filePath))
                {
                    sb.AppendLine(filePath);
                }
                else
                {
                    sb.AppendLine($"{filePath} (File does not exist)");
                }
            }

            return sb.ToString();
        }


        public void LaunchApps()
        {
            foreach (var filePath in FilePaths)
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

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string CachePath => $"./Cache/{_name}";
        public async Task SaveData()
        {
            if (!Directory.Exists("./Cache"))
            {
                Directory.CreateDirectory("./Cache");
            }

            using (var file = File.OpenWrite(CachePath))
            {
                await SaveToStreamAsync(this, file);
            }
        }

        public void DeleteLaunch()
        {
            if (File.Exists(CachePath))
            {
                File.Delete(CachePath);
            }
        }

        private static async Task SaveToStreamAsync(Launch launch, Stream stream)
        {
            await JsonSerializer.SerializeAsync(stream, launch).ConfigureAwait(false);
        }

        public static async Task<Launch> LoadFromStream(Stream stream)
        {
            return (await JsonSerializer.DeserializeAsync<Launch>(stream).ConfigureAwait(false));
        }
        public static async Task<IEnumerable<Launch>> LoadCachedAsync()
        {
            if (!Directory.Exists("./Cache"))
            {
                Directory.CreateDirectory("./Cache");
            }

            var results = new List<Launch>();

            foreach (var file in Directory.EnumerateFiles("./Cache"))
            {
                if (!string.IsNullOrWhiteSpace(new DirectoryInfo(file).Extension)) continue;

                await using var fs = File.OpenRead(file);
                results.Add(await Launch.LoadFromStream(fs).ConfigureAwait(false));
            }

            return results;
        }
    }
}
