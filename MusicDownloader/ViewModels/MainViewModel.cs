using MusicDownloader.Extensions;
using MusicDownloader.Models;
using MusicDownloader.MusicProviders;
using MusicDownloader.Services;
using MusicDownloader.Views;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using YandexMusicApi.Client;
using YandexMusicApi.Client.Http;
using Playlist = YoutubeMusicApi.Models.Playlist;

namespace MusicDownloader.ViewModels;

public sealed class MainViewModel : ViewModelBase
{
    public ICommand SearchPlaylistsCommand { get; }
    public ICommand SearchInDownloadingSourceCommand { get; }
    public ICommand DownloadCommand { get; }

    public string Greeting => "Welcome to Avalonia!";

    public ObservableCollection<Node>? Playlists =>
        _dataToDownload is not null ? new ObservableCollection<Node>(_dataToDownload.Playlists
        .Select(p => new Node(p.Title, new ObservableCollection<Node>(p.Tracks
            .Select(t => new Node(GetTrackNodeTitle(t))))))) : null;

    public bool IsReadyToSearch => _dataToDownload is not null;

    public bool IsReadyToDownload { get; set; }

    public bool IsLoading { get; private set; }


    private Credentials _credentials;
    private DataToDownload? _dataToDownload;
    private YandexMusicClient? _yandexMusicClient;

    public MainViewModel()
    {
        SearchPlaylistsCommand = ReactiveCommand.Create(() => LoadingAction(SearchOriginalPlaylistsAndSetTreeAsync));
        SearchInDownloadingSourceCommand = ReactiveCommand.Create(() => LoadingAction(SearchInDownloadingSourceAsync));
        DownloadCommand = ReactiveCommand.Create(() => LoadingAction(DownloadAsync));

        _credentials = new CredentialsProvider().GetCredentials();
    }

    private async Task SearchOriginalPlaylistsAndSetTreeAsync()
    {
        try
        {
            var provider = new YtMusicProvider(); 
            
            var playlists = await provider.GetPlaylistsAsync(_credentials);

            _dataToDownload = new DataToDownload();
            _dataToDownload.Playlists = playlists
                .Select(p => new PlaylistToDownload 
                {
                    Title = p.Title,
                    Tracks = p.Tracks.Select(t => new TrackToDownload
                    {
                        OriginalAlbum = t.Album,
                        OriginalAuthor = t.Author,
                        OriginalDuration = t.Duration,
                        OriginalName = t.Name
                    }).ToList()
                })
                .ToList();

            UpdateView();
        }
        catch (Exception e)
        {

        }
    }

    private async Task SearchInDownloadingSourceAsync()
    {
        _yandexMusicClient = new YandexMusicClient(RestClient.Authorized("OAuth", _credentials.YandexMusicAuthToken));
        var status = await _yandexMusicClient.Account.GetAccountStatusAsync();

        if (_dataToDownload is null || _dataToDownload.Playlists is null)
        {
            throw new InvalidOperationException();
        }

        foreach (var playlist in _dataToDownload.Playlists)
        {
            foreach (var trackToDownload in playlist.Tracks)
            {
                try
                {
                    var searchQuery = $"{trackToDownload.OriginalAuthor} - {trackToDownload.OriginalName}";

                    if (searchQuery.Contains("qazw33"))
                        searchQuery = searchQuery.Replace("qazw33", string.Empty).Trim(); // Местячковый костыль, чтобы нашлись мои треки из Диско элизиум)))

                    var tracks = await _yandexMusicClient.Tracks.SearchAsync(searchQuery); // Get track by id

                    if (tracks is null || !tracks.Results.Any())
                    {
                        continue; //TODO - попробовать поискать по другому - попробовать без автора,
                                  //распарсить имя, убрать из него слова в скобках (в которых зачастую "feat. ..." и прочая лабуда)
                    }

                    var tracksCollection = (IEnumerable<Track>)tracks.Results;

                    if (trackToDownload.OriginalDuration != default)
                    {
                        var firstResultTrack = tracksCollection.FirstOrDefault();

                        // Sorting by duration will be applied only when first track from result differs in duration significantly (more than 10 secs).
                        if (Math.Abs((TimeSpan.FromMilliseconds(firstResultTrack.DurationMs) - trackToDownload.OriginalDuration).Ticks)
                            > TimeSpan.FromSeconds(10).Ticks)
                        {
                            tracksCollection = tracksCollection
                                .OrderBy(t => Math.Abs((TimeSpan.FromMilliseconds(t.DurationMs) - trackToDownload.OriginalDuration).Ticks));
                        }
                    }

                    var track = tracksCollection.FirstOrDefault(); //TODO - искать ближайший по длительности

                    var album = track.Albums.First();

                    trackToDownload.InternalTrackId = track.Id;
                    trackToDownload.InternalAlbumId = album.Id;
                    trackToDownload.Name = track.Title;
                    trackToDownload.Author = track.Artists.FirstOrDefault().Name;
                    trackToDownload.Album = album.Title;
                    trackToDownload.Duration = TimeSpan.FromMilliseconds(track.DurationMs);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        IsReadyToDownload = true;

        UpdateView();
    }

    private async Task DownloadAsync()
    {
        var downloader = new Downloader();

        await downloader.DownloadAsync(_dataToDownload, _credentials, _yandexMusicClient);
    }

    private async Task LoadingAction(Func<Task> action)
    {
        try
        {
            IsLoading = true;
            UpdateView();

            await action();

            IsLoading = false;
            UpdateView();
        }
        catch (Exception ex)
        {
        }
    }

    private string GetTrackNodeTitle(TrackToDownload track)
    {
        var originalTrackInfo = $"Orig.: {track.OriginalAuthor} {track.OriginalName} {track.OriginalDuration.ToShortString()}";
        if (track.InternalTrackId is null)
        {
            return originalTrackInfo;
        }
        else
        {
            return $"{originalTrackInfo}; Found: {track.Author} {track.Name} {track.Duration.Value.ToShortString()}";
        }
    }

    private void UpdateView()
    {
        OnPropertyChanged(nameof(Playlists));
        OnPropertyChanged(nameof(IsReadyToSearch));
        OnPropertyChanged(nameof(IsReadyToDownload));
        OnPropertyChanged(nameof(IsLoading));
    }
}
