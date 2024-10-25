using MusicDownloader.Models;

namespace MusicDownloader.Services
{
    /// <summary>
    /// Provider of music profile state.
    /// </summary>
    interface IProfileStateProvider
    {
        ProfileState LoadOrCreateProfileState();
    }
}
