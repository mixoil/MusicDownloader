﻿using System;

namespace MusicDownloader.Models.PersistedProfile
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Also, class is using for deserialization to xml.
    /// </remarks>
    public sealed class PersistedTrack
    {
        /// <summary>
        /// Internal track Id in source of downloading.
        /// </summary>
        public string? InternalTrackId { get; set; }

        /// <summary>
        /// Internal track's album Id in source of downloading.
        /// </summary>
        public string? InternalAlbumId { get; set; }

        /// <summary>
        /// Track's name in source of downloading.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Track's author in source of downloading.
        /// </summary>
        public string? Author { get; set; }

        /// <summary>
        /// Track's album name in source of downloading.
        /// </summary>
        public string? Album { get; set; }

        /// <summary>
        /// Track's duration time in source of downloading.
        /// </summary>
        public TimeSpan? Duration { get; set; }

        /// <summary>
        /// Track's name in original music source.
        /// </summary>
        public string OriginalName { get; set; }

        /// <summary>
        /// Track's author in original music source.
        /// </summary>
        public string OriginalAuthor { get; set; }

        /// <summary>
        /// Track's album name in original music source.
        /// </summary>
        public string OriginalAlbum { get; set; }

        /// <summary>
        /// Track's duration time in original music source.
        /// </summary>
        public TimeSpan OriginalDuration { get; set; }
    }
}
