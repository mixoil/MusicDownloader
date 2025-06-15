using System.Collections.Generic;

namespace MusicDownloader.Models.DataToDownload
{
    public sealed class PlaylistToDownload
    {
        public string Title { get; set; }
        public List<TrackToDownload> Tracks { get; set; }
    }
}
