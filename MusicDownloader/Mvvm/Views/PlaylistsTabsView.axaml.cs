using Avalonia.Controls;
using MusicDownloader.Mvvm.ViewModels;
using MusicDownloader.Services;
using Ninject;

namespace MusicDownloader.Mvvm.Views;

public partial class PlaylistsTabsView : UserControl
{
    public PlaylistsTabsView()
    {
        InitializeComponent();

        DataContext = new PlaylistsTabsViewModel(App.Root.Get<IProfileStateProvider>());
    }
}