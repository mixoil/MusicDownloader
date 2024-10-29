using MusicDownloader.Models;
using System.Threading.Tasks;

namespace MusicDownloader.Services
{
    /// <summary>
    /// Provider of music profile state.
    /// </summary>
    public interface IProfileStateProvider
    {
        /// <summary>
        /// Asynchronously loads profile state from file. If file doesn't exist, file will be created.
        /// </summary>
        /// <returns><see cref="Task"/>, providing <see cref="ProfileState"/> instance.</returns>
        Task<ProfileState> LoadOrCreateProfileStateAsync();

        /// <summary>
        /// Determines whether music profile exists and initialized (state file exists).
        /// </summary>
        bool IsProfileInitialized();
    }
}
