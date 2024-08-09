using MusicDownloader.Models;
using System.IO;
using System;
using System.Threading.Tasks;
using YandexMusicApi.Client;
using System.Linq;

namespace MusicDownloader.Services
{
    internal sealed class Downloader : IDownloader
    {
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

                if (Directory.Exists(plFolder))
                {
                    Directory.Delete(plFolder, true); //TODO - делать проверку содержимого
                }
                var dirinfo = Directory.CreateDirectory(plFolder);

                plFolder = dirinfo.FullName; //иногда символы в названии папки обрезаются, поэтому берем действительный путь

                foreach (var (track, i) in playlist.Tracks.Select((t, i) => (t, i)))
                {
                    try
                    {
                        if (track.InternalTrackId is null || track.InternalAlbumId is null)
                        {
                            continue;
                        }

                        var stream = await client.Tracks.DownloadAsync(
                            track.InternalTrackId,
                            track.InternalAlbumId
                            );

                        var trackFileName = $"{track.Author} - {track.Name}.mp3";

                        if (data.DownloadParameters.AddOrderNumberPrefix)
                        {
                            trackFileName = $"{i + 1}. {trackFileName}";
                        }

                        trackFileName = TransformStringToValidWinFilename(trackFileName);

                        var fileStream = File.Create(Path.Combine(plFolder, trackFileName));
                        stream.CopyTo(fileStream);
                        fileStream.Close();
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
    }
}
