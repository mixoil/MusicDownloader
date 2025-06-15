using MusicDownloader.Models.PersistedProfile;
using System.Threading.Tasks;

namespace MusicDownloader.Services
{
    /// <summary>
    /// Provider of music profile state.
    /// </summary>
    public interface IPersistedProfileProvider
    {
        /// <summary>
        /// Asynchronously loads profile state from file.
        /// </summary>
        /// <returns><see cref="Task"/>, providing <see cref="PersistedProfile"/> instance.</returns>
        Task<PersistedProfile?> LoadProfileStateAsync();

        /// <summary>
        /// Creates profile state file and returns it.
        /// </summary>
        /// <returns><see cref="PersistedProfile"/> instance.</returns>
        PersistedProfile? CreateProfileState();

        /// <summary>
        /// Determines whether music profile exists and initialized (state file exists).
        /// </summary>
        bool IsProfileInitialized();
    }
}
