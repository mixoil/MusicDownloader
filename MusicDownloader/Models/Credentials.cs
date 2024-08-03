using System;

namespace MusicDownloader.Models
{
    /// <summary>
    /// TODO!!! And for every field.
    /// </summary>
    internal sealed class Credentials
    {
        [CredentialsProperty("YandexMusicAuthToken", typeof(string))]
        public string YandexMusicAuthToken = string.Empty;

        [CredentialsProperty("YoutubeMusicPlaylistId", typeof(string))]
        public string YoutubeMusicPlaylistId = string.Empty;
    }

    /// <summary>
    /// TODO!!! And for every field.
    /// </summary>
    internal sealed class CredentialsPropertyAttribute : Attribute
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
