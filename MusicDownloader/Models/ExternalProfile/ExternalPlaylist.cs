using System.Collections.Generic;

namespace MusicDownloader.Models.ExternalProfile
{
    public sealed class ExternalPlaylist
    {
        public string Title { get; set; }

        public string PlaylistId { get; set; }

        public string Author { get; set; }

        public string Count { get; set; }

        public string Duration { get; set; }

        public IReadOnlyCollection<ExternalTrack> Tracks { get; set; }

        public string Continuation { get; set; }
    }
}
