using MusicDownloader.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YoutubeMusicApi;
using Playlist = YoutubeMusicApi.Models.Playlist;
namespace MusicDownloader.MusicProviders
{
    /// <inheritdoc cref="IMusicProvider"/>
    public class YtMusicProvider : IMusicProvider
    {
        /// <inheritdoc/>
        public async Task<List<Playlist>> GetPlaylistsAsync(Credentials credentials)
        {
            var playlists = new List<Playlist>();
            // Search for songs directly
            //YouTubeMusicClient client = new();

            //await new YtMusicHelper().Run();

            try
            {

                var client = new YoutubeMusicClient();
                Login(client, credentials);
                var user = await client.GetUser(credentials.YoutubeMusicUserId); //https://music.youtube.com/channel/...
                foreach (var item in user.Playlists)
                {
                    try
                    {
                        var playlist = await client.GetPlaylist(item.PlaylistId, item.Title);
                        playlists.Add(playlist);
                    }
                    catch (Exception ex)
                    {

                    }

                }
                //IEnumerable<CommunityPlaylist> searchResults = await client.SearchAsync<CommunityPlaylist>(query);
                //foreach (CommunityPlaylist song in searchResults)
                //{

                //}
            }
            catch (Exception ex)
            {

            }
            return playlists;
            throw new NotImplementedException();
        }

        private void Login(YoutubeMusicClient client, Credentials credentials)
        {
            //на странице канала в куках в LOGIN_INFO
            var a = client.LoginWithCookie(credentials.YoutubeMusicAuthToken);
        }
    }
}
