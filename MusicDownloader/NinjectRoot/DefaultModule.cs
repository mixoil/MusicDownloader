using MusicDownloader.MusicProviders;
using MusicDownloader.Mvvm.Infrastructure;
using MusicDownloader.Services;
using Ninject.Modules;

namespace MusicDownloader.NinjectRoot
{
    /// <inheritdoc cref="NinjectModule"/>
    public sealed class DefaultModule : NinjectModule
    {
        /// <inheritdoc/>
        public override void Load()
        {
            Bind<IMusicProvider>()
                .To<YtMusicProvider>()
                .InSingletonScope();

            Bind<IViewModelFactory>()
                .To<ViewModelFactory>()
                .InSingletonScope();

            Bind<ICredentialsProvider>()
                .To<CredentialsProvider>()
                .InSingletonScope();

            Bind<IDownloader>()
                .To<Downloader>()
                .InSingletonScope();
        }
    }
}
