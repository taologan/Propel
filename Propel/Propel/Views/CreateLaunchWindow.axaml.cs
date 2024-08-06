using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Propel.ViewModels;
using ReactiveUI;

namespace Propel.Views;

public partial class CreateLaunchWindow : ReactiveWindow<CreateLaunchViewModel>
{
    public CreateLaunchWindow()
    {
        InitializeComponent();
        this.WhenActivated(action => action(ViewModel!.CreateLaunchCommand.Subscribe(Close)));
    }
}