﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YoutubeMusicApi.Models.Generated;

namespace YoutubeMusicApi.Models
{
    public class Playlist
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("playlistId")]
        public string PlaylistId { get; set; }

        [JsonProperty("thumbnails")]
        public List<Thumbnail> Thumbnails { get; set; }

        [JsonProperty("author")]
        public IdNamePair Author { get; set; }

        [JsonProperty("count")]
        public string Count { get; set; }

        [JsonProperty("privacy")]
        public PrivacyStatus Privacy { get; set; }

        [JsonProperty("duration")]
        public string Duration { get; set; }

        [JsonProperty("tracks")]
        public List<PlaylistTrack> Tracks { get; set; } = new List<PlaylistTrack>();

        [JsonProperty("continuation")]
        public string Continuation { get; set; }


        /// <summary>
        /// Note that this is what is returned when getting lists of playlists and
        /// only includes some of the fields of the playlist (like Title, PlaylistId, Thumbnails, Count).
        /// To get the full playlist information you have to call GetPlaylist
        /// </summary>
        /// <param name="renderer"></param>
        /// <returns></returns>
        public static Playlist FromMusicTwoRowItemRenderer(MusicTwoRowItemRenderer renderer)
        {
            Playlist playlist = new Playlist();

            playlist.PlaylistId = renderer.NavigationEndpoint.BrowseEndpoint.BrowseId;
            playlist.Thumbnails = renderer.ThumbnailRenderer.MusicThumbnailRenderer.Thumbnail.Thumbnails;
            playlist.Title = renderer.Title.Runs[0].Text;
            
            if (renderer.Subtitle.Runs.Count >= 3)
            {
                playlist.Count = renderer.Subtitle.Runs[2].Text;
            }

            return playlist;
        }

        public static Playlist FromRoot(YoutubeMusicApi.Models.AutoGenerated.Root root, string title)
        {
            try
            {
                Playlist playlist = new Playlist();
                var plInfo = root.contents.TwoColumnBrowseResultsRenderer.Tabs.FirstOrDefault()!.TabRenderer
                    .Content.SectionListRenderer.Contents.FirstOrDefault()!.MusicResponsiveHeaderRenderer.SecondSubtitle.runs;

                playlist.Title = title;
                playlist.Count = plInfo.FirstOrDefault(i => i.Text.Contains("tracks")).Text.Replace("tracks", string.Empty).Trim();
                playlist.Duration = plInfo.FirstOrDefault(i => i.Text.Contains("hours") || i.Text.Contains("minutes") || i.Text.Contains("hour") || i.Text.Contains("minute"))
                    .Text.Trim();

                var tracksInfo = root.contents.TwoColumnBrowseResultsRenderer.SecondaryContents.SectionListRenderer.Contents.FirstOrDefault()!.MusicPlaylistShelfRenderer.Contents;

                foreach (var track in tracksInfo)
                {
                    var plTrack = new PlaylistTrack();

                    var trackInfo = track.MusicResponsiveListItemRenderer;
                    plTrack.Duration = TimeSpan.Parse(track.MusicResponsiveListItemRenderer.FixedColumns.FirstOrDefault()!.MusicResponsiveListItemFixedColumnRenderer
                        .Text.Runs.FirstOrDefault()!.Text.Trim());
                    var cols = track.MusicResponsiveListItemRenderer.FlexColumns;
                    plTrack.Name = cols.FirstOrDefault()!.MusicResponsiveListItemFlexColumnRenderer.Text.Runs.FirstOrDefault()!.Text.Trim();
                    plTrack.Author = cols.Skip(1).FirstOrDefault().MusicResponsiveListItemFlexColumnRenderer.Text.Runs.FirstOrDefault()!.Text.Trim();
                    plTrack.Album = cols.Skip(2)?.FirstOrDefault()?.MusicResponsiveListItemFlexColumnRenderer?.Text?.Runs?.FirstOrDefault()?.Text?.Trim();

                    playlist.Tracks.Add(plTrack);
                }

                //playlist.PlaylistId = renderer.NavigationEndpoint.BrowseEndpoint.BrowseId;
                //playlist.Thumbnails = renderer.ThumbnailRenderer.MusicThumbnailRenderer.Thumbnail.Thumbnails;
                //playlist.Title = renderer.Title.Runs[0].Text;

                //if (renderer.Subtitle.Runs.Count >= 3)
                //{
                //    playlist.Count = renderer.Subtitle.Runs[2].Text;
                //}

                return playlist;
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        //public static Playlist FromBrowseResponse(BrowseResponse response)
        //{
        //    Playlist playlist = new Playlist();

        //    var contents = response.Contents.SingleColumnBrowseResultsRenderer.Tabs[0].TabRenderer.Content.SectionListRenderer.Contents[0].MusicPlaylistShelfRenderer;

        //    playlist.PlaylistId = contents.PlaylistId;
        //    bool isUserPlaylist = response.Header.MusicEditablePlaylistDetailHeaderRenderer != null;
        //    Header header = null;
        //    if (isUserPlaylist)
        //    {
        //        header = response.Header.MusicEditablePlaylistDetailHeaderRenderer.Header;
        //    }
        //    else
        //    {
        //        header = response.Header;// not sure if right
        //    }

        //    if (header.MusicDetailHeaderRenderer != null && header.MusicDetailHeaderRenderer.Privacy != null)
        //    {
        //        playlist.Privacy = (PrivacyStatus)Enum.Parse(typeof(PrivacyStatus), header.MusicDetailHeaderRenderer.Privacy, true);
        //    }
        //    else
        //    {
        //        playlist.Privacy = PrivacyStatus.Public;
        //    }

        //    var authorRuns = header.MusicDetailHeaderRenderer.Subtitle.Runs;
        //    if (authorRuns[2].NavigationEndpoint == null)
        //    {
        //        // sometimes the author is "YouTube Music"
        //        playlist.Author = new IdNamePair(authorRuns[2].Text, authorRuns[2].Text);
        //    }
        //    else
        //    {
        //        playlist.Author = new IdNamePair(authorRuns[2].NavigationEndpoint.BrowseEndpoint.BrowseId, authorRuns[2].Text);
        //    }

        //    playlist.Title = header.MusicDetailHeaderRenderer.Title.Runs[0].Text;

        //    playlist.Thumbnails = header.MusicDetailHeaderRenderer.Thumbnail.CroppedSquareThumbnailRenderer.Thumbnail.Thumbnails;

        //    var secondSubtitleRuns = header.MusicDetailHeaderRenderer.SecondSubtitle.Runs;
        //    playlist.Count = secondSubtitleRuns[0].Text;
        //    if (secondSubtitleRuns.Count >= 3)
        //    {
        //        playlist.Duration = secondSubtitleRuns[2].Text;
        //    }

        //    if (contents.Contents != null)
        //    {
        //        contents.Contents.ForEach(x =>
        //            playlist.Tracks.Add(PlaylistTrack.FromMusicResponsiveListItemRenderer(x.MusicResponsiveListItemRenderer))
        //        );
        //    }

        //    if (contents.Continuations != null)
        //    {
        //        playlist.Continuation = contents.Continuations[0].NextContinuationData.Continuation;
        //    }

        //    return playlist;
        //}
    }

    public class PlaylistTrack
    {
        public string Name { get; set; }
        public string Author { get; set; }
        public string Album { get; set; }
        public TimeSpan Duration { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum PrivacyStatus
    { 
        Public,
        Private,
        Unlisted,
    }

}