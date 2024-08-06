using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;
using Propel.Models;
using Propel.Views;
using ReactiveUI;

namespace Propel.ViewModels;

public class MainViewModel : ViewModelBase
{
    public ICommand CreateNewLaunch { get; }
    public ReactiveCommand<Unit,Unit> RemoveLaunch { get; }
    public Interaction<CreateLaunchViewModel,LaunchViewModel?> ShowDialog { get; }

    public ObservableCollection<LaunchViewModel> Launches { get; } = new();
    private Dictionary<string, LaunchViewModel> LaunchMap { get; } = new();

    private string _launchNameToRemove;
    public string LaunchNameToRemove
    {
        get => _launchNameToRemove;
        set => this.RaiseAndSetIfChanged(ref _launchNameToRemove, value);
    }

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
                Launches.Add(result);
                LaunchMap[result.Name] = result;
                Console.WriteLine("yay");
                await result.SaveToDisc();
            }
            else
            {
                Console.WriteLine("not yay");
            }
        });
        RemoveLaunch = ReactiveCommand.Create(removeLaunch);
    }

    public void removeLaunch()
    {
        // Console.WriteLine(_launchNameToRemove);
        if (LaunchMap.TryGetValue(_launchNameToRemove, out var launchViewModel))
        {
            Launches.Remove(launchViewModel);
            launchViewModel.DeleteLaunch();
            // Console.WriteLine("DEL");
        }
        // Console.WriteLine("NODEL");
    }

    public async void LoadData()
    {
        var launches = (await Launch.LoadCachedAsync()).Select(x => new LaunchViewModel(x));
        foreach (var launch in launches)
        {
            Launches.Add(launch);
            LaunchMap[launch.Name] = launch;
        }
    }
}