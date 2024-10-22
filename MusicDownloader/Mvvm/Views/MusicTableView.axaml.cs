using Avalonia.Controls;
using MusicDownloader.Mvvm.ViewModels;

namespace MusicDownloader.Mvvm.Views;

public partial class MusicTableView : UserControl
{
    public MusicTableView()
    {
        InitializeComponent();

        DataContext = new MusicTableViewModel();
    }
}