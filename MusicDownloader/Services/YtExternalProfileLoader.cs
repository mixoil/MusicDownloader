using MusicDownloader.Models;
using MusicDownloader.Models.ExternalProfile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YoutubeMusicApi;
using YoutubeMusicApi.Models;

namespace MusicDownloader.Services
{
    /// <summary>
    /// 
    /// </summary>
    // TODO: move to Yt csproj
    public sealed class YtExternalProfileLoader : IExternalProfileLoader
    {
        private readonly ICredentialsProvider _credentialsProvider;

        public YtExternalProfileLoader(ICredentialsProvider credentialsProvider)
        {
            _credentialsProvider = credentialsProvider ?? throw new ArgumentNullException(nameof(ICredentialsProvider));
        }

        public async Task<ExternalProfile> LoadProfileAsync()
        {
            var res = new ExternalProfile();
            var playlists = new List<Playlist>();

            try
            {
                var creds = _credentialsProvider.GetCredentials();

                var client = new YoutubeMusicClient();
                Login(client, creds);
                var user = await client.GetUser(creds.YoutubeMusicUserId); //https://music.youtube.com/channel/...
                res.Name = user.Name;

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
            }
            catch (Exception ex)
            {

            }

            //TODO: add AutoMapper
            res.Playlists = playlists.Select(
                p => new ExternalPlaylist {
                        Author = p.Author.Name,
                        Continuation = p.Continuation,
                        Count = p.Count,
                        Duration = p.Duration,
                        PlaylistId = p.PlaylistId,
                        Title = p.Title,
                        Tracks = p.Tracks.Select(t => 
                            new ExternalTrack {
                                Album = t.Album,
                                Author = t.Author,
                                Duration = t.Duration,
                                Name = t.Name
                            }).ToArray()
                    }
                ).ToArray();

            return res;
        }

        private void Login(YoutubeMusicClient client, Credentials credentials)
        {
            //на странице канала в куках в LOGIN_INFO
            var a = client.LoginWithCookie(credentials.YoutubeMusicAuthToken);
        }
    }
}
