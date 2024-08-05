using System.Collections.ObjectModel;
using Propel.Models;
using ReactiveUI;

namespace Propel.ViewModels;

public class LaunchViewModel : ViewModelBase
{
    private readonly Launch _launch;

    public LaunchViewModel(Launch launch)
    {
        _launch = launch;
    }

    public string PathString => _launch.ToString();
    private string name => _launch.Name;
    private ObservableCollection<string> _filePaths => _launch.FilePaths;
}