using System.Collections.Generic;

namespace MusicDownloader.Models.PersistedProfile
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Also, class is using for deserialization to xml.
    /// </remarks>
    public sealed class PersistedPlaylist
    {
        public string Title { get; set; }

        public List<PersistedTrack> Tracks { get; set; }
    }
}
