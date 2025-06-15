using System;
using System.Collections.Generic;
namespace MusicDownloader.Models.PersistedProfile
{
    /// <summary>
    /// Current state of folder, containing playlist subfolders (with tracks files).
    /// Also contains downloading settings. App will generate file describing this state in folder, that contains playlist subfolders.
    /// </summary>
    /// <remarks>
    /// Also, class is using for deserialization to xml.
    /// </remarks>
    public sealed class PersistedProfile
    {
        /// <summary>
        /// Download parameters.
        /// </summary>
        public DownloadParameters Parameters { get; set; }

        /// <summary>
        /// Playlist folders.
        /// </summary>
        public List<PersistedPlaylist> PlaylistStates { get; set; }

        public PersistedProfile(
            List<PersistedPlaylist> playlistStates,
            DownloadParameters parameters
            )
        {
            PlaylistStates = playlistStates ?? throw new ArgumentNullException(nameof(playlistStates));
            Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
        }

        /// <summary>
        /// Empty constructor for serializer.
        /// </summary>
        public PersistedProfile()
        {
            PlaylistStates = [];
            Parameters = new();
        }

        /// <summary>
        /// Creates empty profile state.
        /// </summary>
        public static PersistedProfile CreateEmpty()
        {
            return new PersistedProfile(new List<PersistedPlaylist> { new PersistedPlaylist { Title = "Test playlist title", Tracks = [
                new PersistedTrack { Album = "Test alb", Author = "Test aut", Duration = new TimeSpan(0, 2, 1), InternalAlbumId = "00", InternalTrackId = "01", Name = "Test track name", OriginalAlbum = "Orig test alb", OriginalAuthor = "Orig test aut", OriginalDuration = new TimeSpan(0, 2, 1), OriginalName = " Orig test track name" }
                ] } }, new DownloadParameters());
        }
    }
}
