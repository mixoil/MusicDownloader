using MusicDownloader.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MusicDownloader.MusicProviders
{
    /// <summary>
    /// Music source.
    /// </summary>
    public interface IMusicProvider
    {
        /// <summary>
        /// Get all sounds from link.
        /// </summary>
        Task<List<Sound>> GetSoundsAsync(string link);
    }
}
