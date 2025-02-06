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
        /// Asynchronously loads profile state from file.
        /// </summary>
        /// <returns><see cref="Task"/>, providing <see cref="ProfileState"/> instance.</returns>
        Task<ProfileState?> LoadProfileStateAsync();

        /// <summary>
        /// Creates profile state file and returns it.
        /// </summary>
        /// <returns><see cref="ProfileState"/> instance.</returns>
        ProfileState? CreateProfileState();

        /// <summary>
        /// Determines whether music profile exists and initialized (state file exists).
        /// </summary>
        bool IsProfileInitialized();
    }
}
