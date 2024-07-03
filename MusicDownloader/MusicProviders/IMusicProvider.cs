using MusicDownloader.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using YoutubeMusicApi.Models;

namespace MusicDownloader.MusicProviders
{
    /// <summary>
    /// Music source.
    /// </summary>
    public interface IMusicProvider
    {
        /// <summary>
        /// Get all playlist from link.
        /// </summary>
        Task<List<Playlist>> GetPlaylistsAsync(string link);
    }
}
