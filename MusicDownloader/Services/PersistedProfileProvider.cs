using MusicDownloader.Models.PersistedProfile;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MusicDownloader.Services
{
    /// <inheritdoc cref="IPersistedProfileProvider"/>
    public class PersistedProfileProvider : IPersistedProfileProvider
    {
        private const string StateFileName = "ProfileState.xml";

        private readonly ICredentialsProvider _credentialsProvider;

        public PersistedProfileProvider(ICredentialsProvider credentialsProvider)
        {
            _credentialsProvider = credentialsProvider ?? throw new ArgumentNullException(nameof(credentialsProvider));
        }

        /// <inheritdoc/>
        public async Task<PersistedProfile?> LoadProfileStateAsync()
        {
            try
            {
                PersistedProfile? res = null;

                if (!IsProfileInitialized())
                {
                    throw new InvalidOperationException("Profile state is not initialized.");
                }

                var folderPath = GetDownloadingFolderPath();
                var stateFilePath = Path.Combine(folderPath, StateFileName);

                using (FileStream fs = new FileStream(stateFilePath, FileMode.Open))
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(PersistedProfile));
                    res = await Task.Run(() => xmlSerializer.Deserialize(fs) as PersistedProfile);
                }

                return res;
            }
            catch (Exception ex)
            {
                //log
                return null;
            }
        }

        /// <inheritdoc/>
        public PersistedProfile? CreateProfileState()
        {
            try
            {
                if (!IsProfileInitialized())
                {
                    return InitializeProfile();
                }

                return null;
            }
            catch (Exception ex)
            {
                //log
                return null;
            }
        }

        /// <inheritdoc/>
        public bool IsProfileInitialized()
        {
            try
            {
                var folderPath = GetDownloadingFolderPath();

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
            catch (Exception ex)
            {
                //log
                return false;
            }
        }

        /// <summary>
        /// Creates profile folder if not exists and creates state file.
        /// </summary>
        private PersistedProfile? InitializeProfile()
        {
            PersistedProfile? res = null;

            var folderPath = GetDownloadingFolderPath();

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var stateFilePath = Path.Combine(folderPath, StateFileName);

            if (!File.Exists(stateFilePath))
            {
                using var stream = File.Create(stateFilePath);

                res = PersistedProfile.CreateEmpty();

                XmlSerializer xmlSerializer = new XmlSerializer(typeof(PersistedProfile));
                xmlSerializer.Serialize(stream, res);
            }

            return res;
        }

        /// <summary>
        /// Returns downloading folder's path.
        /// </summary>
        private string GetDownloadingFolderPath()
        {
            var credentials = _credentialsProvider.GetCredentials();

            return credentials.DownloadingFolderPath;
        }
    }
}
