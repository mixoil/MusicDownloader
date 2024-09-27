namespace MusicDownloader.Mvvm.Infrastructure
{
    public interface IViewModelFactory
    {
        IViewModel Create<TModel>(TModel model)
            where TModel : ModelBase;
    }
}
