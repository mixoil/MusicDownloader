using System.Collections.Generic;

namespace MusicDownloader.Models
{
    internal sealed class PlaylistToDownload
    {
        public List<TrackToDownload> Tracks { get; set; }
    }
}
