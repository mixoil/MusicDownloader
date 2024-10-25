using System;
using System.Collections.Generic;
namespace MusicDownloader.Models
{
    /// <summary>
    /// "Music profile state" (or just "Profile state") - current state of folder, containing playlist subfolders (with tracks files).
    /// Also contains downloading settings. App will generate file describing this state in folder, that contains playlist subfolders.
    /// </summary>
    public sealed class ProfileState
    {
        /// <summary>
        /// Download parameters.
        /// </summary>
        public DownloadParameters Parameters { get; set; }

        /// <summary>
        /// Playlist folders.
        /// </summary>
        public IList<PlaylistState> PlaylistStates { get; set; }

        public ProfileState(IList<PlaylistState> playlistStates)
        {
            PlaylistStates = playlistStates ?? throw new ArgumentNullException(nameof(playlistStates));
        }
    }
}
