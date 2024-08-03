using MusicDownloader.Entities;
using MusicDownloader.Models;
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
        /// Get all playlist from provided credentials.
        /// </summary>
        public Task<List<Playlist>> GetPlaylistsAsync(Credentials credentials);
    }
}
