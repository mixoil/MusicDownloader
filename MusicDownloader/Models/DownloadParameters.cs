namespace MusicDownloader.Models
{
    public sealed class DownloadParameters
    {
        /// <summary>
        /// Add to downloaded track name order number to retain tracks order from original playlist.
        /// </summary>
        public bool AddOrderNumberPrefix { get; set; } = true; // TODO: add checkbox to view
    }
}
