using MusicDownloader.Models;

namespace MusicDownloader.Services
{
    /// <summary>
    /// Provider of credentials from configuration .xml file.
    /// </summary>
    public interface ICredentialsProvider
    {
        /// <summary>
        /// Returns <see cref="Credentials"/> filled from configuration .xml file.
        /// </summary>
        Credentials GetCredentials();
    }
}
