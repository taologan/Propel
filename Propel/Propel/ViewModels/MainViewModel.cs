using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Input;
using Propel.Models;
using Propel.Views;
using ReactiveUI;

namespace Propel.ViewModels;

public class MainViewModel : ViewModelBase
{
    public ICommand CreateNewLaunch { get; }
    public Interaction<CreateLaunchViewModel,LaunchViewModel?> ShowDialog { get; }

    public ObservableCollection<LaunchViewModel> Launches { get; } = new();
    public MainViewModel()
    {
        LoadData();
        ShowDialog = new Interaction<CreateLaunchViewModel, LaunchViewModel?>();
        CreateNewLaunch = ReactiveCommand.CreateFromTask(async() =>
        {
            var form = new CreateLaunchViewModel();
            var result = await ShowDialog.Handle(form);
            if (result != null)
            {
                Console.WriteLine("yay");
                await result.SaveToDisc();
                Launches.Add(result);
            }
            else
            {
                Console.WriteLine("not yay");
            }
        });
    }

    public async void LoadData()
    {
        var launches = (await Launch.LoadCachedAsync()).Select(x => new LaunchViewModel(x));
        foreach (var launch in launches)
        {
            Launches.Add(launch);
        }
    }
}