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
            return new ProfileState(new List<PlaylistState>(), new DownloadParameters());
        }
    }
}
