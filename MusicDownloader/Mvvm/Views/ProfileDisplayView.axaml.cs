using Avalonia.Controls;
using MusicDownloader.Mvvm.ViewModels;

namespace MusicDownloader.Mvvm.Views;

public partial class ProfileDisplayView : UserControl
{
    public ProfileDisplayView()
    {
        InitializeComponent();

        DataContext = new ProfileDisplayViewModel();
    }
}