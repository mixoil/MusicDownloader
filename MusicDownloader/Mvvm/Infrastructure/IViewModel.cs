namespace MusicDownloader.Mvvm.Infrastructure
{
    public interface IViewModel
    {
        void OnInitialize();


        void OnFinalize();


        bool IsShown { get; }
    }
}
