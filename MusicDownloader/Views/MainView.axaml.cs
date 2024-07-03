using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Microsoft.Extensions.DependencyInjection;
using MusicDownloader.MusicProviders;
using System.Linq;
using VkNet.Model;
using VkNet;
using VkNet.AudioBypassService.Extensions;
using System.Text.RegularExpressions;
using VkNet.Enums.Filters;
using System.Net.Http;
using YoutubeMusicApi.Models;
using VkNet.Utils.AntiCaptcha;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using YandexMusicApi.Client;
using YandexMusicApi.Client.Http;
namespace MusicDownloader.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
    }

    public async void ClickHandler(object sender, RoutedEventArgs args)
    {
        //var dialog = new IStorageProvider();
        //var result = await dialog.ShowAsync(this);

        var folder = "";

        var provider = new YtMusicProvider();
        var playls = await provider.GetPlaylistsAsync("https://music.youtube.com/playlist?list=...");
        tracks.ItemsSource = playls.Select(t => $"{t.Title} - {t.Author} - {t.PlaylistId} - {t.Count}");
            
        //var a = new Yandex.Music.Api.YandexMusicApi();
        //a.Track.D()


        foreach (var sound in playls)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        var client = new YandexMusicClient(RestClient.Authorized("OAuth", "y0_Ag..."));
        var s = await client.Account.GetAccountStatusAsync();
        foreach (var playl in playls)
        {
            
            var plFolder = Path.Combine(folder, playl.Title);

            if (Directory.Exists(plFolder))
            {
                Directory.Delete(plFolder, true); //TODO - делать проверку содержимого
            }
            var dirinfo = Directory.CreateDirectory(plFolder);

            plFolder = dirinfo.FullName; //иногда символы в названии папки обрезаются, поэтому берем действительный путь

            foreach (var sound in playl.Tracks)
            {
                try
                {
                    //client.
                    //var acc = await client.Account.GetAccountSettingsAsync();

                    var tracks = await client.Tracks.SearchAsync($"{sound.Author} - {sound.Name}"); // get track by id

                    if (!tracks.Results.Any())
                    {
                        continue; //TODO - попробовать поискать по другому - попробовать без автора,
                                  //распарсить имя, убрать из него слова в скобках (в которых зачастую "feat. ..." и прочая лабуда)
                    }

                    var track = tracks.Results.FirstOrDefault(); //TODO - искать ближайший по длительности

                    var stream = await client.Tracks.DownloadAsync(track.Id, track.Albums.FirstOrDefault().Id);
                    var fileStream = File.Create(Path.Combine(plFolder, $"{sound.Author} - {sound.Name}.mp3"));
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


}
