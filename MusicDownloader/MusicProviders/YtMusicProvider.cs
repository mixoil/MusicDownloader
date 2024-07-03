﻿using Google.Apis.YouTube.v3.Data;
using MusicDownloader.Entities;
using MusicDownloader.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YoutubeMusicApi;
using YoutubeMusicApi.Models;
using YoutubeMusicApi.Models.AutoGenerated;
using YoutubeMusicApi.Models.Generated;
using Playlist = YoutubeMusicApi.Models.Playlist;
//using YouTubeMusicAPI.Client;
//using YouTubeMusicAPI.Models;
namespace MusicDownloader.MusicProviders
{
    /// <inheritdoc cref="IMusicProvider"/>
    internal class YtMusicProvider : IMusicProvider
    {
        /// <inheritdoc/>
        public async Task<List<Playlist>> GetPlaylistsAsync(string link)
        {
            var playlists = new List<Playlist>();
            // Search for songs directly
            //YouTubeMusicClient client = new();

            //await new YtMusicHelper().Run();

            try
            {

                var client = new YoutubeMusicClient();
                Login(client);
                var user = await client.GetUser("..."); //https://music.youtube.com/channel/...
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

        private void Login(YoutubeMusicClient client)
        {
            //на странице канала в куках в LOGIN_INFO
            var a = client.LoginWithCookie("");
        }
    }
}
