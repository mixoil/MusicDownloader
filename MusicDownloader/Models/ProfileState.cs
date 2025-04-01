using System;
using System.Collections.Generic;
namespace MusicDownloader.Models
{
    /// <summary>
    /// "Music profile state" (or just "Profile state") - current state of folder, containing playlist subfolders (with tracks files).
    /// Also contains downloading settings. App will generate file describing this state in folder, that contains playlist subfolders.
    /// </summary>
    /// <remarks>
    /// Also, class is using for deserialization to xml.
    /// </remarks>
    public sealed class ProfileState
    {
        /// <summary>
        /// Download parameters.
        /// </summary>
        public DownloadParameters Parameters { get; set; }

        /// <summary>
        /// Playlist folders.
        /// </summary>
        public List<PlaylistState> PlaylistStates { get; set; }

        public ProfileState(
            List<PlaylistState> playlistStates,
            DownloadParameters parameters
            )
        {
            PlaylistStates = playlistStates ?? throw new ArgumentNullException(nameof(playlistStates));
            Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
        }

        /// <summary>
        /// Empty constructor for serializer.
        /// </summary>
        public ProfileState()
        {
            PlaylistStates = [];
            Parameters = new();
        }

        /// <summary>
        /// Creates empty profile state.
        /// </summary>
        public static ProfileState CreateEmpty()
        {
            return new ProfileState(new List<PlaylistState> { new PlaylistState { Title = "Test playlist title", Tracks = [
                new TrackState { Album = "Test alb", Author = "Test aut", Duration = new TimeSpan(0, 2, 1), InternalAlbumId = "00", InternalTrackId = "01", Name = "Test track name", OriginalAlbum = "Orig test alb", OriginalAuthor = "Orig test aut", OriginalDuration = new TimeSpan(0, 2, 1), OriginalName = " Orig test track name" }
                ] } }, new DownloadParameters());
        }
    }
}
