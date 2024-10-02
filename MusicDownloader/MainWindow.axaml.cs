using Avalonia.Controls;
using System;

namespace MusicDownloader;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void OnOpened(object? sender, EventArgs e)
    {
        InitializeApp();
    }

    private void OnClosed(object? sender, EventArgs e)
    {

    }

    private void InitializeApp()
    {
        App.InitializeRoot();
    }
}
