using System.Collections.Generic;

namespace MusicDownloader.Models
{
    /// <summary>
    /// Data to download.
    /// </summary>
    internal sealed class DataToDownload
    {
        /// <summary>
        /// Playlists.
        /// </summary>
        public List<PlaylistToDownload> Playlists { get; set; }

        /// <summary>
        /// Downloading parameters set.
        /// </summary>
        public DownloadParameters DownloadParameters { get; set; } = new();
    }
}
