using Avalonia.Controls;
using System.Collections.ObjectModel;

namespace MusicDownloader.Mvvm.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
    }
}

public class Node
{
    public ObservableCollection<Node>? SubNodes { get; }
    public string Title { get; }

    public Node(string title)
    {
        Title = title;
    }

    public Node(string title, ObservableCollection<Node> subNodes)
    {
        Title = title;
        SubNodes = subNodes;
    }
}
