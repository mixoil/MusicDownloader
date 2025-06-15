using System;

namespace MusicDownloader.Models.ExternalProfile
{
    public sealed class ExternalTrack
    {
        public string Name { get; set; }

        public string Author { get; set; }

        public string Album { get; set; }

        public TimeSpan Duration { get; set; }
    }
}
