﻿using System.Collections.Generic;

namespace MusicDownloader.Models
{
    public sealed class PlaylistToDownload
    {
        public string Title { get; set; }
        public List<TrackToDownload> Tracks { get; set; }
    }
}
