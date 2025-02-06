using MusicDownloader.Models;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;

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
        public async Task<ProfileState?> LoadProfileStateAsync()
        {
            try
            {
                ProfileState? res = null;

                if (!IsProfileInitialized())
                {
                    throw new InvalidOperationException("Profile state is not initialized.");
                }

                var folderPath = GetDownloadingFolderPath();
                var stateFilePath = Path.Combine(folderPath, StateFileName);

                using (FileStream fs = new FileStream(stateFilePath, FileMode.Open))
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(ProfileState));
                    res = await Task.Run(() => xmlSerializer.Deserialize(fs) as ProfileState);
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
        public ProfileState? CreateProfileState()
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
        private ProfileState? InitializeProfile()
        {
            ProfileState? res = null;

            var folderPath = GetDownloadingFolderPath();

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var stateFilePath = Path.Combine(folderPath, StateFileName);

            if (!File.Exists(stateFilePath))
            {
                using var stream = File.Create(stateFilePath);

                res = ProfileState.CreateEmpty();

                XmlSerializer xmlSerializer = new XmlSerializer(typeof(ProfileState));
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
