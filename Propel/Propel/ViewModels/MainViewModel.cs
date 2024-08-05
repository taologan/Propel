using System.Reactive.Linq;
using System.Windows.Input;
using Propel.Views;
using ReactiveUI;

namespace Propel.ViewModels;

public class MainViewModel : ViewModelBase
{
    public ICommand CreateNewLaunch { get; }
    public Interaction<CreateLaunchViewModel,LaunchViewModel?> ShowDialog { get; }

    public MainViewModel()
    {
        ShowDialog = new Interaction<CreateLaunchViewModel, LaunchViewModel?>();
        CreateNewLaunch = ReactiveCommand.CreateFromTask(async() =>
        {
            var form = new CreateLaunchViewModel();
            var result = await ShowDialog.Handle(form);
        });
    }   
}