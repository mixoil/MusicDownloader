using MusicDownloader.Models;
using System.IO;
using System;
using System.Threading.Tasks;
using YandexMusicApi.Client;
using System.Linq;
using Path = System.IO.Path;
using System.Collections.Generic;
using MusicDownloader.Models.DataToDownload;

namespace MusicDownloader.Services
{
    public sealed class Downloader : IDownloader
    {
        private const string DownloadingFormat = ".mp3";

        public async Task DownloadAsync(DataToDownload data, Credentials credentials, YandexMusicClient client) //TODO: add Ninject, move credentials to DI container
        {
            var folder = credentials.DownloadingFolderPath;

            var s = await client.Account.GetAccountStatusAsync();


            foreach (var playlist in data.Playlists)
            {
                if (!playlist.Tracks.Any())
                {
                    continue;
                }

                var plFolder = Path.Combine(folder, playlist.Title);

                //if (!Directory.Exists(plFolder))
                //{
                //    //Directory.Delete(plFolder, true); //TODO - делать проверку содержимого
                //}
                var dirinfo = Directory.CreateDirectory(plFolder);

                plFolder = dirinfo.FullName; //иногда символы в названии папки обрезаются, поэтому берем действительный путь

                var existingTrackIds = GetExistingTrackIds(plFolder);

                var indexFormat = new string('0', GetNumberBitDebpth(playlist.Tracks.Count));

                foreach (var (track, i) in playlist.Tracks.Select((t, i) => (t, i)))
                {
                    try
                    {
                        if (track.InternalTrackId is null 
                            || track.InternalAlbumId is null
                            || existingTrackIds.Contains(track.InternalTrackId)
                            )
                        {
                            continue;
                        }

                        var stream = await client.Tracks.DownloadAsync(
                            track.InternalTrackId,
                            track.InternalAlbumId
                            );

                        var trackFileName = $"{track.Name}{DownloadingFormat}";

                        if (data.DownloadParameters.AddOrderNumberPrefix)
                        {
                            trackFileName = $"{(i + 1).ToString(indexFormat)}. {trackFileName}";
                        }

                        trackFileName = TransformStringToValidWinFilename(trackFileName);

                        var trackFilePath = Path.Combine(plFolder, trackFileName);

                        var fileStream = File.Create(trackFilePath);
                        stream.CopyTo(fileStream);
                        fileStream.Close();

                        AddTrackMetadata(trackFilePath, track);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
        }

        private string TransformStringToValidWinFilename(string str)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                str = str.Replace(c, '_');
            }
            return str;
        }

        /// <remarks><paramref name="num"/> supposed to be positive.</remarks>
        private int GetNumberBitDebpth(int num)
        {
            var depth = 0;

            while (num > 0)
            {
                depth++;
                num = num / 10;
            }

            return depth;
        }

        private void AddTrackMetadata(string trackPath, TrackToDownload track)
        {
            TagLib.File f = TagLib.File.Create(trackPath);
            f.Tag.Title = track.Name;
            f.Tag.Album = track.Album;
            f.Tag.Performers = [track.Author];
            
            f.Tag.MusicIpId = track.InternalTrackId;
            f.Save();
        }

        private HashSet<string> GetExistingTrackIds(string folder)
        {
            var result = new HashSet<string>();

            var existingTracksPaths = Directory
                .EnumerateFiles(folder)
                .Where(p => p.EndsWith(DownloadingFormat));

            foreach (var trackPath in existingTracksPaths)
            {
                TagLib.File f = TagLib.File.Create(trackPath);

                if (!string.IsNullOrWhiteSpace(f.Tag.MusicIpId))
                {
                    result.Add(f.Tag.MusicIpId);
                }
            }

            return result;
        }
    }
}
