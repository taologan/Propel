using System.Collections.ObjectModel;
using System.Threading.Tasks;
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
    public string Name => _launch.Name;
    private ObservableCollection<string> FilePaths => _launch.FilePaths;

    public void LaunchApps()
    {
        _launch.LaunchApps();
    }
    public async Task SaveToDisc()
    {
        await _launch.SaveData();
    }

    public void DeleteLaunch()
    {
        _launch.DeleteLaunch();
    }

}