﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using YoutubeMusicApi.Logging;
using YoutubeMusicApi.Models;
using YoutubeMusicApi.Models.Generated;
using YoutubeMusicApi.Models.Search;

namespace YoutubeMusicApi
{
    public class YoutubeMusicClient
    {

        public AuthHeaders AuthHeaders { get; private set; }
        public ILogger Logger { get; set; }

        public YoutubeMusicClient(ILogger logger = null)
        {
            // init to default values w/o a cookie
            AuthHeaders = new AuthHeaders();
            Logger = logger;
        }

        #region Authentication

        public async Task<bool> LoginWithAuthJsonFile(string filePath)
        {
            using (StreamReader reader = File.OpenText(filePath))
            {
                string contents = await reader.ReadToEndAsync();
                return  LoginWithAuthHeaderString(contents);
            }
        }

        public bool LoginWithAuthHeaderString(string headersJson)
        {
            AuthHeaders = JsonConvert.DeserializeObject<AuthHeaders>(headersJson);

            return !string.IsNullOrEmpty(AuthHeaders.Cookie);
        }

        public bool LoginWithCookie(string cookie)
        {
            AuthHeaders = new AuthHeaders
            {
                Cookie = cookie
            };

            return true;
        }

        public bool IsAuthed()
        {
            return AuthHeaders != null && !string.IsNullOrEmpty(AuthHeaders.Cookie);
        }

        #endregion

        #region Search

        public async Task<SearchResult> Search(string search, SearchResultType filter = SearchResultType.All)
        {
            if (filter == SearchResultType.Upload)
            {
                throw new Exception("To search uploads, use SearchUploads() method. Uploads will not be returned as a result from Search()");
            }

            string url = GetYTMUrl("search");

            JObject data = JObject.FromObject(new
            {
                query = search
            });

            if (filter != SearchResultType.All)
            {
                string parameters = GetSearchParamStringFromFilter(filter);
                data.Add("params", parameters);
            }

            BrowseResponse response = await Post<BrowseResponse>(url, data);

            return SearchResult.FromBrowseResponse(response, filter);
        }

        /// <summary>
        /// Search uploaded songs, artists, albums
        /// 
        /// The YTM api does not return results with mixes of uploads and non uploaded tracks, so you
        /// have to search for uploads specifically
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public async Task<SearchResult> SearchUploads(string search)
        {
            string url = GetYTMUrl("search");

            JObject data = JObject.FromObject(new
            {
                query = search
            });

            string parameters = GetSearchParamStringFromFilter(SearchResultType.Upload);
            data.Add("params", parameters);

            BrowseResponse response = await Post<BrowseResponse>(url, data, authRequired: true);

            return SearchResult.FromBrowseResponse(response, SearchResultType.Upload);
        }

        private string GetSearchParamStringFromFilter(SearchResultType filter)
        {
            string param1 = "Eg-KAQwIA";
            string param3 = "MABqChAEEAMQCRAFEAo%3D";
            string parameters = "";

            if (filter == SearchResultType.Upload)
            {
                parameters = "agIYAw%3D%3D";
            }
            else
            {
                string param2 = "";
                switch (filter)
                {
                    case SearchResultType.Album:
                        param2 = "BAAGAEgACgA";
                        break;
                    case SearchResultType.Artist:
                        param2 = "BAAGAAgASgA";
                        break;
                    case SearchResultType.Playlist:
                        param2 = "BAAGAAgACgB";
                        break;
                    case SearchResultType.Song:
                        param2 = "RAAGAAgACgA";
                        break;
                    case SearchResultType.Video:
                        param2 = "BABGAAgACgA";
                        break;
                    case SearchResultType.Upload:
                        param2 = "RABGAEgASgB"; // not sure if this is right, uploads should never get here due to above if clause, but the python code has this
                        break;
                    default:
                        throw new Exception($"Unsupported search filter type: {filter}");
                }

                parameters = param1 + param2 + param3;
            }

            return parameters;
        }
        #endregion

        #region Browsing

        /// <summary>
        /// Gets the artist with provided id
        /// </summary>
        /// <param name="id">artist id</param>
        /// <param name="authRequired">Use if you want to get subscribed information</param>
        /// <returns></returns>
        public async Task<Artist> GetArtist(string id, bool authRequired=false)
        {
            string url = GetYTMUrl("browse");

            var data = PrepareBrowse("ARTIST", id);

            var response = await Post<BrowseResponse>(url, data, authRequired: authRequired);

            return Artist.FromBrowseResponse(response);
        }

        public async Task<JObject> GetArtistAlbums(string channelId, string parameters)
        {
            // TODO: is this API even needed?
            // albums are returned by getartist, not sure what the params arg is...
            throw new NotImplementedException();
        }

        public async Task<User> GetUser(string channelId)
        {
            string url = GetYTMUrl("browse");

            var data = JObject.FromObject(new
            {
                browseId = channelId
            });

            var response = await Post<BrowseResponse>(url, data);

            return User.FromBrowseResponse(response);
        }

        // TODO: Not tested
        public async Task<JObject> GetAlbum(string browseId)
        {
            string url = GetYTMUrl("browse");

            var data = PrepareBrowse("ALBUM", browseId);

            var response = await Post<JObject>(url, data);

            return response;
        }

        public async Task<Song> GetSong(string videoId)
        {
            string url = $"https://www.youtube.com/get_video_info?video_id={videoId}&el=detailpage&hl=en";

            var result = await GetFromQueryStringEndpoint(url);

            return Song.FromNameValueCollection(result);
        }

        #endregion

        #region Library

        public async Task<PlaylistList> GetLibraryPlaylists(string continuation=null)
        {
            string url = GetYTMUrl("browse", continuation);

            var data = JObject.FromObject(new
            {
                browseId = "FEmusic_liked_playlists"
            });

            var response = await Post<BrowseResponse>(url, data, authRequired: true);

            return PlaylistList.FromBrowseResponse(response);
        }

        // TODO: Not Tested
        public async Task<JObject> GetLibrarySongs(string continuation=null)
        {
            string url = GetYTMUrl("browse", continuation);

            var data = JObject.FromObject(new
            {
                browseId = "FEmusic_liked_videos"
            });

            var response = await Post<JObject>(url, data, authRequired: true);

            return response;
        }

        // TODO: Not Tested
        public async Task<JObject> GetLibraryAlbums(string continuation=null)
        {
            
            string url = GetYTMUrl("browse", continuation);

            var data = JObject.FromObject(new
            {
                browseId = "FEmusic_liked_albums"
            });

            var response = await Post<JObject>(url, data, authRequired: true);

            return response;
        }

        // TODO: Not Tested
        public async Task<JObject> GetLibraryArtists(string continuation=null)
        {
            string url = GetYTMUrl("browse", continuation);

            var data = JObject.FromObject(new
            {
                browseId = "FEmusic_library_corpus_track_artists"
            });

            var response = await Post<JObject>(url, data, authRequired: true);

            return response;
        }

        // TODO: Not Tested
        public async Task<JObject> GetLibrarySubscriptions(string continuation=null)
        {
            string url = GetYTMUrl("browse", continuation);

            var data = JObject.FromObject(new
            {
                browseId = "FEmusic_library_corpus_artists"
            });

            var response = await Post<JObject>(url, data, authRequired: true);

            return response;
        }

        // TODO: Not Tested
        //public async Task<Playlist> GetLikedSongs(string continuation=null)
        //{
        //    return await GetPlaylist("LM", continuation: continuation);
        //}

        // TODO: Not Tested
        public async Task<JObject> GetHistory()
        {
            string url = GetYTMUrl("browse");

            var data = JObject.FromObject(new
            {
                browseId = "FEmusic_history"
            });

            var response = await Post<JObject>(url, data, authRequired: true);

            return response;
        }


        // TODO: Not Tested
        public async Task<JObject> RemoveHistoryItems(List<string> feedbackTokens)
        {
            string url = GetYTMUrl("feedback");

            var data = JObject.FromObject(new
            {
                feedbackTokens = feedbackTokens
            });

            var response = await Post<JObject>(url, data, authRequired: true);

            return response;
        }

        // TODO: Not Tested
        public async Task<JObject> RateSong(string videoId, LikeStatus rating)
        {
            string url = GetYTMUrl(PrepareLikeEndpoint(rating));

            var data = JObject.FromObject(new
            {
                target = JObject.FromObject(new
                {
                    videoId = videoId
                })
            });

            var response = await Post<JObject>(url, data, authRequired: true);

            return response;
        }
        
        // TODO: Not Tested
        public async Task<JObject> RatePlaylist(string playlistId, LikeStatus rating)
        {
            string url = GetYTMUrl(PrepareLikeEndpoint(rating));

            var data = JObject.FromObject(new
            {
                target = JObject.FromObject(new
                {
                    playlistId = playlistId
                })
            });

            var response = await Post<JObject>(url, data, authRequired: true);

            return response;
        }

        // TODO: Not Tested
        public async Task<JObject> SubscribeArtists(List<string> channelIds)
        {
            string url = GetYTMUrl("subscription/subscribe");

            var data = JObject.FromObject(new
            {
                channelIds = channelIds
            });

            var response = await Post<JObject>(url, data, authRequired: true);

            return response;
        }

        // TODO: Not Tested
        public async Task<JObject> UnsubscribeArtists(List<string> channelIds)
        {
            string url = GetYTMUrl("subscription/unsubscribe");

            var data = JObject.FromObject(new
            {
                channelIds = channelIds
            });

            var response = await Post<JObject>(url, data, authRequired: true);

            return response;
        }

        #endregion

        #region Playlists

        public async Task<Playlist> GetPlaylist(string id, string title, string continuation = null, bool authRequired = false)
        {
            var browseId = id.StartsWith("PL") ? id : id.StartsWith("VL") ? id : $"VL{id}";
            string url = GetYTMUrl("browse", continuation);
            var data = PrepareBrowse("PLAYLIST", browseId);
            
            var response = await Post<YoutubeMusicApi.Models.AutoGenerated.Root>(url, data, authRequired: authRequired);

            return Playlist.FromRoot(response, title);
        }

        //public async Task<Playlist> GetPlaylistV3(string id, string continuation = null, bool authRequired = false)
        //{
        //    var browseId = id.StartsWith("PL") ? id : id.StartsWith("VL") ? id : $"VL{id}";
        //    string url = GetYTMUrl("browse", continuation);
        //    var data = PrepareBrowse("PLAYLIST", browseId);

        //    var response = await Post<BrowseResponse>(url, data, authRequired: authRequired);

        //    return Playlist.FromBrowseResponse(response);
        //}

        public async Task<string> CreatePlaylist(string title, string description, PrivacyStatus privacyStatus = PrivacyStatus.Private, List<string> videoIds = null, string sourcePlaylist = null)
        {
            string url = GetYTMUrl("playlist/create");
            var data = JObject.FromObject(new
            {
                title = title,
                description = description,
                privacyStatus = privacyStatus,
            });

            if (videoIds != null)
            {
                data.Merge(JObject.FromObject(new
                {
                    videoIds = videoIds
                }));
            }

            if (sourcePlaylist != null)
            {
                data.Merge(JObject.FromObject(new
                {
                    sourcePlaylist = sourcePlaylist
                }));
            }

            var response = await Post<BrowseResponse>(url, data, authRequired: true);
            return response.PlaylistId;
        }

        // TODO: Not tested
        public async Task<JObject> EditPlaylist(string playlistId, string title = null, string description = null, PrivacyStatus? privacyStatus = null, Tuple<string, string> moveItem = null, string addPlaylistId = null)
        {
            string url = GetYTMUrl("browse/edit_playlist");

            List<JObject> actionsList = new List<JObject>();

            if (title != null)
            {
                actionsList.Add(JObject.FromObject(new
                {
                    action = "ACTION_SET_PLAYLIST_NAME",
                    playlistName = title
                }));
            }

            if (description != null)
            {
                actionsList.Add(JObject.FromObject(new
                {
                    action = "ACTION_SET_PLAYLIST_DESCRIPTION",
                    playlistDescription = description
                }));
            }

            if (privacyStatus != null)
            {
                actionsList.Add(JObject.FromObject(new
                {
                    action = "ACTION_SET_PLAYLIST_PRIVACY",
                    playlistPrivacy = privacyStatus
                }));
            }

            if (moveItem != null)
            {
                actionsList.Add(JObject.FromObject(new
                {
                    action = "ACTION_MOVE_VIDEO_BEFORE",
                    setVideoId = moveItem.Item1,
                    moveSetVideoIdSuccessor = moveItem.Item2
                }));
            }

            if (addPlaylistId != null)
            {
                actionsList.Add(JObject.FromObject(new
                {
                    action = "ACTION_ADD_PLAYLIST",
                    addedFullListId = addPlaylistId
                }));
            }

            var data = JObject.FromObject(new
            {
                playlistId = playlistId,
                actions = actionsList,
            });

            return await Post<JObject>(url, data, authRequired: true);
        }

        public async Task<bool> DeletePlaylist(string playlistId)
        {
            string url = GetYTMUrl("playlist/delete");
            var data = JObject.FromObject(new
            {
                playlistId = playlistId
            });
            var response = await Post<BrowseResponse>(url, data, authRequired: true);

            bool success = response.Command != null
                && response.Command.HandlePlaylistDeletionCommand != null
                && response.Command.HandlePlaylistDeletionCommand.PlaylistId != null;

            return success;
        }

        public async Task<bool> AddPlaylistItems(string playlistId, List<string> videoIds, string sourcePlaylist=null, bool duplicates=false)
        {
            var browseId = TrimPlaylistIdForEdit(playlistId);
            string url = GetYTMUrl("browse/edit_playlist");

            List<JObject> actionsList = new List<JObject>();

            foreach (var video in videoIds)
            {
                var action = JObject.FromObject(new
                {
                    action = "ACTION_ADD_VIDEO",
                    addedVideoId = video,
                });

                if (duplicates)
                {
                    action.Add("dedupeOption", JToken.FromObject("DEDUPE_OPTION_SKIP"));
                }

                actionsList.Add(action);
            }

            if (sourcePlaylist != null)
            {
                actionsList.Add(JObject.FromObject(new
                {
                    action = "ACTION_ADD_PLAYLIST",
                    addedFullListId = sourcePlaylist
                }));
            }

            var data = JObject.FromObject(new
            {
                playlistId = browseId,
                actions = actionsList,
            });

            var response = await Post<BrowseResponse>(url, data, authRequired: true);

            // TODO: update BrowseResponse to have fields this endpoint returns
            // like newHeader, actions
            // also consider returning more info than just a bool
            return response.Status == "STATUS_SUCEEDED";
        }

        //public async Task<bool> RemovePlaylistItems(string playlistId,  List<PlaylistTrack> videos)
        //{
        //    var browseId = TrimPlaylistIdForEdit(playlistId);
        //    string url = GetYTMUrl("browse/edit_playlist");

        //    List<JObject> actionsList = new List<JObject>();

        //    foreach (var video in videos)
        //    {
        //        if (video.SetVideoId == null)
        //        {
        //            throw new Exception($"Cannot remove song from playlist that you don't have permission to. PlaylistId={playlistId}, video={video.VideoId}");
        //        }

        //        actionsList.Add(JObject.FromObject(new
        //        {
        //            setVideoId = video.SetVideoId,
        //            removeVideoId = video.VideoId,
        //            action = "ACTION_REMOVE_VIDEO",
        //        }));
        //    }

        //    var data = JObject.FromObject(new
        //    {
        //        playlistId = browseId,
        //        actions = actionsList,
        //    });

        //    // TODO: update BrowseResponse to have fields this endpoint returns
        //    // like newHeader, actions
        //    // also consider returning more info than just a bool
        //    var response = await Post<BrowseResponse>(url, data, authRequired: true);

        //    return response.Status == "STATUS_SUCCEEDED";
        //}

        #endregion

        #region Uploads

        // TODO: Not tested
        public async Task<JObject> GetUploadedSongs(string continuation = null)
        {
            string url = GetYTMUrl("browse", continuation: continuation);

            var data = JObject.FromObject(new
            {
                browseId = "FEmusic_library_privately_owned_tracks"
            });

            var response = await Post<JObject>(url, data, authRequired: true);

            return response;
        }

        // TODO: Not tested
        public async Task<JObject> GetUploadedAlbums(string continuation = null)
        {
            string url = GetYTMUrl("browse", continuation: continuation);

            var data = JObject.FromObject(new
            {
                browseId = "FEmusic_library_privately_owned_releases"
            });

            var response = await Post<JObject>(url, data, authRequired: true);

            return response;
        }

        // TODO: Not tested
        public async Task<JObject> GetUploadedArtists(string continuation = null)
        {
            string url = GetYTMUrl("browse", continuation: continuation);

            var data = JObject.FromObject(new
            {
                browseId = "FEmusic_library_privately_owned_artists"
            });

            var response = await Post<JObject>(url, data, authRequired: true);

            return response;
        }

        // TODO: Not tested
        public async Task<JObject> GetUploadedArtist(string artistBrowseId)
        {
            string url = GetYTMUrl("browse");

            var data = PrepareBrowse("ARTIST", artistBrowseId);

            var response = await Post<JObject>(url, data, authRequired: true);

            return response;
        }

        // TODO: Not tested
        public async Task<JObject> GetUploadedAlbum(string albumBrowseId)
        {
            string url = GetYTMUrl("browse");

            var data = PrepareBrowse("ALBUM", albumBrowseId);

            var response = await Post<JObject>(url, data, authRequired: true);

            return response;
        }

        public async Task<JObject> UploadSong(string filePath)
        {
            throw new NotImplementedException();
        }

        // TODO: Not tested
        public async Task<JObject> DeleteUpload(string entityId)
        {
            string url = GetYTMUrl("music/delete_privately_owned_entity");

            entityId.Replace("FEmusic_library_privately_owned_release_detail", "");

            var data = JObject.FromObject(new
            {
                entityId = entityId
            });

            var response = await Post<JObject>(url, data, authRequired: true);

            return response;
        }


        #endregion

        #region Requests

        private async Task<NameValueCollection> GetFromQueryStringEndpoint(string url, bool authRequired = false)
        {
            if (authRequired && !IsAuthed())
            {
                throw new Exception("Tried to make a GET request that requires auth but was not authed");
            }

            HttpClient client = GetHttpClient(authRequired: authRequired);

            Log($"GET: {url}");
            var response = await client.GetAsync(url);
            var responseString = await response.Content.ReadAsStringAsync();
            Log($"\tRESPONSE: {responseString}");
            return HttpUtility.ParseQueryString(responseString);
        }

        private async Task<T> Get<T>(string url, bool authRequired = false)
        {
            HttpClient client = GetHttpClient(authRequired: authRequired);

            Log($"GET: {url}");
            var response = await client.GetAsync(url);
            var responseString = await response.Content.ReadAsStringAsync();
            Log($"\tRESPONSE: {responseString}");
            T result = JsonConvert.DeserializeObject<T>(responseString);
            return result;
        }

        private async Task<T> Post<T>(string url, JObject data,  bool authRequired = false)
        {
            if (authRequired && !IsAuthed())
            {
                throw new Exception("Tried to make a POST request that requires auth but was not authed");
            }

            HttpClient client = GetHttpClient(authRequired: authRequired);

            data.Merge(YoutubeMusicClientConstants.DefaultBody);
            string requestBody = data.ToString();
            HttpContent content = new StringContent(requestBody, System.Text.Encoding.UTF8, "application/json");

            Log($"POST: {url}");
            Log($"\tBODY: {requestBody}");
            var response = await client.PostAsync(url, content);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Request failed with response: {responseString}");
            }

            Log($"\tRESPONSE: {responseString}");
            T result = JsonConvert.DeserializeObject<T>(responseString);
            return result;
        }

        private HttpClient GetHttpClient(bool authRequired = false)
        {
            var httpClientHandler = new HttpClientHandler() { UseCookies = false };
            HttpClient client = new HttpClient(httpClientHandler);

            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("User-Agent", AuthHeaders.UserAgent);
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(AuthHeaders.Accept));
            client.DefaultRequestHeaders.Add("Accept-Language", AuthHeaders.AcceptLanguage);
            client.DefaultRequestHeaders.Add("X-Goog-AuthUser", AuthHeaders.GoogAuthUser);
            client.DefaultRequestHeaders.Add("x-origin", AuthHeaders.Origin);
            client.DefaultRequestHeaders.Add("Cookie", AuthHeaders.Cookie);

            if (authRequired)
            {
                string auth = GetAuthorization(AuthHeaders.Cookie, AuthHeaders.Origin);
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("SAPISIDHASH", auth);
            }

            return client;
        }

        #endregion

        #region Utils

        private string GetAuthorization(string cookie, string origin)
        {
            string sapisid = GetSapsidFromCookie(cookie);
            string timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();
            string authToEncode = $"{timestamp} {sapisid} {origin}";
            byte[] bytes = Encoding.UTF8.GetBytes(authToEncode);
            var sha = SHA1.Create();
            byte[] digest = sha.ComputeHash(bytes);
            string decoded = String.Concat(Array.ConvertAll(digest, x => x.ToString("X2").ToLower()));
            string auth = $"{timestamp}_{decoded}";
            return auth;
        }

        private string GetSapsidFromCookie(string cookie)
        {
            string pattern = "SAPISID=(?<sapisid>[^;]+);";
            Regex regex = new Regex(pattern);
            Match match = regex.Match(cookie);
            string sapisid = match.Groups["sapisid"].Value;
            return sapisid;
        }

        private string GetYTMUrl(string endpoint, string continuation = null)
        {
            string url = $"{YoutubeMusicClientConstants.BaseUrl}/{endpoint}{YoutubeMusicClientConstants.Params}";
            if (continuation != null)
            {
                url += $"&ctoken={continuation}&continuation={continuation}";
            }

            return url;
        }

        private string PrepareLikeEndpoint(LikeStatus status)
        {
            switch (status)
            {
                case LikeStatus.Like:
                    return "like/like";
                case LikeStatus.Dislike:
                    return "like/dislike";
                case LikeStatus.Indifferent:
                    return "like/removelike";
                default:
                    throw new Exception($"Unsupported likestatus value {status}");
            }

        }

        private JObject PrepareBrowse(string endpoint, string id)
        {
            return JObject.FromObject(new
            {
                browseEndpointContextSupportedConfigs = new
                {
                    browseEndpointContextMusicConfig = new
                    {
                        pageType = $"MUSIC_PAGE_TYPE_{endpoint}"
                    }
                },
                browseId = id
            });
        }

        private void Log(string str)
        {
            if (Logger != null)
            {
                Logger.Log(str);
            }
        }

        private string TrimPlaylistIdForEdit(string playlistId)
        {
            return playlistId.StartsWith("VL") ? playlistId.Substring(2, playlistId.Length-1) : playlistId;
        }

        #endregion

    }
}
