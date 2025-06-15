using System.Collections.Generic;

namespace MusicDownloader.Models.ExternalProfile
{
    public sealed class ExternalProfile
    {
        public string Name { get; set; }

        public IReadOnlyCollection<ExternalPlaylist> Playlists { get; set; }
    }
}
