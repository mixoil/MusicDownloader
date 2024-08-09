using System;

namespace MusicDownloader.Extensions
{
    /// <summary>
    /// <see cref="TimeSpan"/> struct extensions.
    /// </summary>
    internal static class TimeSpanExtensions
    {
        /// <summary>
        /// Returns <see cref="TimeSpan"/> string representation
        /// in format "hh:mm:ss" or "mm:ss" if hour component is zero.
        /// </summary>
        public static string ToShortString(this TimeSpan value)
        {
            return value.Hours != 0 ? value.ToString("hh\\:mm\\:ss") : value.ToString("mm\\:ss");
        }
    }
}
