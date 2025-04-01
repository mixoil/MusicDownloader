using Avalonia.Controls;
using MusicDownloader.Mvvm.ViewModels;
using MusicDownloader.Services;
using Ninject;

namespace MusicDownloader.Mvvm.Views;

public partial class ProfileDisplayView : UserControl
{
    public ProfileDisplayView()
    {
        InitializeComponent();

        DataContext = new ProfileDisplayViewModel(App.Root.Get<IProfileStateProvider>());
    }
}