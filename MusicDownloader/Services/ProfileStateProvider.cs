using MusicDownloader.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MusicDownloader.Services
{
    /// <inheritdoc cref="IProfileStateProvider"/>
    public class ProfileStateProvider : IProfileStateProvider
    {
        private const string StateFileName = "ProfileState.xml";

        private readonly ICredentialsProvider _credentialsProvider;

        public ProfileStateProvider(ICredentialsProvider credentialsProvider)
        {
            _credentialsProvider = credentialsProvider ?? throw new ArgumentNullException(nameof(credentialsProvider));
        }

        /// <inheritdoc/>
        public async Task<ProfileState> LoadOrCreateProfileStateAsync()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool IsProfileInitialized()
        {
            var credentials = _credentialsProvider.GetCredentials();

            var folderPath = credentials.DownloadingFolderPath;

            if (!Directory.Exists(folderPath))
            {
                return false;
            }

            var stateFilePath = Path.Combine(folderPath, StateFileName);

            if (!File.Exists(stateFilePath))
            {
                return false;
            }

            return true;
        }
    }
}
