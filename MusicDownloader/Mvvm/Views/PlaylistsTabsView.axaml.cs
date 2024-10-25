using Avalonia.Controls;
using MusicDownloader.Mvvm.ViewModels;

namespace MusicDownloader.Mvvm.Views;

public partial class PlaylistsTabsView : UserControl
{
    public PlaylistsTabsView()
    {
        InitializeComponent();

        DataContext = new PlaylistsTabsViewModel();
    }
}