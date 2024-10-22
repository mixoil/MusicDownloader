using System;
using System.Collections.Generic;
namespace MusicDownloader.Models
{
    /// <summary>
    /// Current state of folder, containing playlist subfolders. Also contains downloading settings.
    /// App will save this state as XML file in folder.
    /// </summary>
    public sealed class MusicFolderState
    {
        public DownloadParameters Parameters { get; set; }

        /// <summary>
        /// Playlist folders.
        /// </summary>
        public IList<PlaylistState> PlaylistStates { get; set; }

        public MusicFolderState(IList<PlaylistState> playlistStates)
        {
            PlaylistStates = playlistStates ?? throw new ArgumentNullException(nameof(playlistStates));
        }
    }
}
