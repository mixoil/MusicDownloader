using MusicDownloader.Models;
using MusicDownloader.MusicProviders;
using MusicDownloader.Services;
using MusicDownloader.Views;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using YandexMusicApi.Client;
using YandexMusicApi.Client.Http;
using Playlist = YoutubeMusicApi.Models.Playlist;

namespace MusicDownloader.ViewModels;

public class MainViewModel : ViewModelBase
{
    public ICommand SearchPlaylistsCommand { get; }
    public ICommand DownloadCommand { get; }

    public string Greeting => "Welcome to Avalonia!";

    public ObservableCollection<Node>? Playlists =>
        _playlists is not null ? new ObservableCollection<Node>(_playlists
        .Select(p => new Node(p.Title, new ObservableCollection<Node>(p.Tracks
            .Select(t => new Node(t.Author + " " + t.Name)))))) : null;

    public bool IsReadyToDownload => Playlists is not null;

    private Credentials _credentials;
    private List<Playlist>? _playlists;

    public MainViewModel()
    {
        SearchPlaylistsCommand = ReactiveCommand.Create(SetPlaylistsTreeAsync);
        DownloadCommand = ReactiveCommand.Create(DownloadAsync);

        _credentials = new CredentialsProvider().GetCredentials();
    }

    public async Task SetPlaylistsTreeAsync()
    {
        try
        {
            var provider = new YtMusicProvider(); 
            
            _playlists = await provider.GetPlaylistsAsync(_credentials);

            UpdateView();
        }
        catch (Exception e)
        {

        }
    }

    private async Task DownloadAsync()
    {
        var folder = _credentials.DownloadingFolderPath;

        var client = new YandexMusicClient(RestClient.Authorized("OAuth", _credentials.YandexMusicAuthToken));
        var s = await client.Account.GetAccountStatusAsync();


        foreach (var playl in _playlists)
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

    private void UpdateView()
    {
        OnPropertyChanged(nameof(Playlists));
        OnPropertyChanged(nameof(IsReadyToDownload));
    }
}
