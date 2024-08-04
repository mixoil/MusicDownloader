using ReactiveUI;
using System.ComponentModel;

namespace MusicDownloader.ViewModels;

public class ViewModelBase : ReactiveObject, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
