using System.Collections.Generic;

namespace MusicDownloader.Models.AggregatedProfile
{
    // TODO: "aggregated" models must be ViewModels
    public sealed class AggregatedPlaylist
    {
        public string Title { get; set; }

        public string ExternalId { get; set; }

        public IReadOnlyCollection<AggregatedTrack> Tracks { get; set; }
    }
}
