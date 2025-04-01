using System.Collections.Generic;

namespace MusicDownloader.Models
{
    public sealed class PlaylistState
    {
        public string Title { get; set; }

        public List<TrackState> Tracks { get; set; }
    }
}
