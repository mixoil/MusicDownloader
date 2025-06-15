using System.Collections.Generic;

namespace MusicDownloader.Models.AggregatedProfile
{
    public sealed class AggregatedProfile
    {
        public IReadOnlyCollection<AggregatedPlaylist> Playlists { get; set; }
    }
}
