using System;

namespace MusicDownloader.Models
{
    /// <summary>
    /// TODO!!! And for every field.
    /// </summary>
    public sealed class Credentials
    {
        /// <summary>
        /// How to get token: https://github.com/MarshalX/yandex-music-api/discussions/513#discussioncomment-2729781
        /// </summary>
        [CredentialsProperty("YandexMusicAuthToken", typeof(string))]
        public string YandexMusicAuthToken = string.Empty;

        [CredentialsProperty("YoutubeMusicAuthToken", typeof(string))]
        public string YoutubeMusicAuthToken = string.Empty;

        [CredentialsProperty("YoutubeMusicUserId", typeof(string))]
        public string YoutubeMusicUserId = string.Empty;

        [CredentialsProperty("DownloadingFolderPath", typeof(string))]
        public string DownloadingFolderPath = string.Empty;
    }

    /// <summary>
    /// TODO!!! And for every field.
    /// </summary>
    public sealed class CredentialsPropertyAttribute : Attribute
    {
        public readonly string Name;
        public readonly Type PropType;

        public CredentialsPropertyAttribute(string name, Type propType)
        {
            Name = name;
            PropType = propType;
        }
    }
}
