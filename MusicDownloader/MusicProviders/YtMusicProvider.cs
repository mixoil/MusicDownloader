using MusicDownloader.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YouTubeMusicAPI.Client;
using YouTubeMusicAPI.Models;
namespace MusicDownloader.MusicProviders
{
    /// <inheritdoc cref="IMusicProvider"/>
    internal class YtMusicProvider : IMusicProvider
    {
        /// <inheritdoc/>
        public async Task<List<Sound>> GetSoundsAsync(string link)
        {// Search for songs directly
            YouTubeMusicClient client = new();

            var query = "https://music.youtube.com/playlist?list=PLiey0xV3jJt157P9WZIQmwRYRc9ECGDrP";

            IEnumerable<CommunityPlaylist> searchResults = await client.SearchAsync<CommunityPlaylist>(query);
            foreach (CommunityPlaylist song in searchResults)
            {

            }
        }
    }
}
