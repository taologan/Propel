using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using Propel.ViewModels;
using ReactiveUI;

namespace Propel.Views;

public partial class MainWindow : ReactiveWindow<MainViewModel>
{
    public MainWindow()
    {
        InitializeComponent();
        this.WhenActivated(action => action(ViewModel!.ShowDialog.RegisterHandler(DoShowDialogAsync)));
    }
    private async Task DoShowDialogAsync(InteractionContext<CreateLaunchViewModel,
        LaunchListViewModel?> interaction)
    {
        var dialog = new CreateLaunchWindow();
        dialog.DataContext = interaction.Input;
    
        var result = await dialog.ShowDialog<LaunchListViewModel?>(this);
        interaction.SetOutput(result);
    }
    
}