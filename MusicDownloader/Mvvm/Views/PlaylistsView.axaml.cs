using Avalonia.Controls;
using MusicDownloader.Mvvm.ViewModels;
using MusicDownloader.Services;
using Ninject;

namespace MusicDownloader.Mvvm.Views;

public partial class PlaylistsView : UserControl
{
    public PlaylistsView()
    {
        InitializeComponent();

        DataContext = new PlaylistsViewModel(App.Root.Get<IProfileStateProvider>());
    }
}