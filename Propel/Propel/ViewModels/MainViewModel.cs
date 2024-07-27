using System.Reactive.Linq;
using System.Windows.Input;
using ReactiveUI;

namespace Propel.ViewModels;

public class MainViewModel : ViewModelBase
{
    public ICommand CreateNewLaunch { get; }
    public Interaction<CreateLaunchViewModel,LaunchListViewModel?> ShowDialog { get; }

    public MainViewModel()
    {
        ShowDialog = new Interaction<CreateLaunchViewModel, LaunchListViewModel?>();
        CreateNewLaunch = ReactiveCommand.CreateFromTask(async() =>
        {
            var form = new CreateLaunchViewModel();
            var result = await ShowDialog.Handle(form);
        });
    }   
}