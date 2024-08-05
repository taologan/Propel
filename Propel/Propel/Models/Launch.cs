using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;

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
    }
}
