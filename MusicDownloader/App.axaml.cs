﻿using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using MusicDownloader.Mvvm.ViewModels;
using MusicDownloader.Mvvm.Views;
using MusicDownloader.NinjectRoot;
using Ninject;

namespace MusicDownloader;

public partial class App : Application
{
    public static StandardKernel Root;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        InitializeRoot();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainViewModel()
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = new MainViewModel()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    public static void InitializeRoot()
    {
        Root = RootFactory.CreateRoot();
    }
}
