using System.Collections.Generic;

namespace MusicDownloader.Models
{
    /// <summary>
    /// Data to download.
    /// </summary>
    internal sealed class DataToDownload
    {
        public List<PlaylistToDownload> Playlists { get; set; }
    }
}
