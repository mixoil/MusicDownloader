using MusicDownloader.Models.AggregatedProfile;
using MusicDownloader.Models.ExternalProfile;
using MusicDownloader.Mvvm.Infrastructure;
using MusicDownloader.Services;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;

namespace MusicDownloader.Mvvm.ViewModels
{
    public sealed class PlaylistsViewModel : ViewModelBase
    {
        /// <summary>
        /// Initialize profile state command.
        /// </summary>
        public ReactiveCommand<Unit, Unit> InitProfileStateCommand { get; }

        /// <summary>
        /// External tracks loading command.
        /// </summary>
        public ReactiveCommand<Unit, Unit> LoadExternalProfileCommand { get; }
        
        /// <summary>
        /// Whether the folder is initialized.
        /// </summary>
        public bool IsMusicFolderInitialized => _profileStateProvider.IsProfileInitialized();

        private readonly IPersistedProfileProvider _profileStateProvider;
        private readonly IExternalProfileLoader _externalProfileLoader;
        private readonly IAggregatedStateProvider _aggregatedStateProvider;

        private AggregatedProfile? _aggregatedProfile;

        private AggregatedPlaylist? _currentPlaylist;

        private ExternalProfile? _externalProfile;

        public List<PlaylistBtnViewModel>? PlaylistBtns => _aggregatedProfile?.Playlists
            .Select(p => new PlaylistBtnViewModel(p, SelectPlaylist))
            .ToList();

        public List<TrackViewModel>? Tracks => _currentPlaylist?.Tracks?
            .Select(t => new TrackViewModel(t))
            .ToList();

        public string? CurrentPlaylist => _currentPlaylist?.Title;

        /// <summary>
        /// Constructor for designer.
        /// </summary>
#pragma warning disable CS8618
        public PlaylistsViewModel() { }
#pragma warning restore CS8618

        public PlaylistsViewModel(
            IPersistedProfileProvider profileStateProvider,
            IExternalProfileLoader externalProfileLoader,
            IAggregatedStateProvider aggregatedStateProvider
            )
        {
            _profileStateProvider = profileStateProvider ?? throw new ArgumentNullException(nameof(profileStateProvider));
            _externalProfileLoader = externalProfileLoader ?? throw new ArgumentNullException(nameof(externalProfileLoader));
            _aggregatedStateProvider = aggregatedStateProvider ?? throw new ArgumentNullException(nameof(aggregatedStateProvider));

            InitProfileStateCommand = ReactiveCommand.Create(() =>
            {
                _ = _profileStateProvider.CreateProfileState();
                OnPropertyChanged(nameof(IsMusicFolderInitialized));
            });

            LoadExternalProfileCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                _externalProfile = await _externalProfileLoader.LoadProfileAsync();
                _aggregatedProfile = await _aggregatedStateProvider.MixPersistedWithExternalAsync(_externalProfile);
                OnPropertyChanged(nameof(PlaylistBtns));
            });

            LoadProfile();
        }

        private void LoadProfile()
        {
            if (_profileStateProvider.IsProfileInitialized())
            {
                Task.Run(async () =>
                {
                    _aggregatedProfile = await _aggregatedStateProvider.LoadPersistedAsync();
                    OnPropertyChanged(nameof(PlaylistBtns));
                });
            }
        }

        private void SelectPlaylist(AggregatedPlaylist playlist)
        {
            _currentPlaylist = playlist;
            OnPropertyChanged(nameof(CurrentPlaylist));
            OnPropertyChanged(nameof(Tracks));
        }
    }

    public sealed class PlaylistBtnViewModel : ViewModelBase
    {
        public ReactiveCommand<Unit, Unit> SelectPlaylistCommand { get; }

        public string BtnTitle => _playlist.Title;

        private readonly Action<AggregatedPlaylist> _playlistSettingAction;
        private readonly AggregatedPlaylist _playlist;

        public PlaylistBtnViewModel(AggregatedPlaylist playlist, Action<AggregatedPlaylist> playlistSettingAction)
        {
            _playlist = playlist ?? throw new ArgumentNullException(nameof(playlist));
            _playlistSettingAction = playlistSettingAction ?? throw new ArgumentNullException(nameof(playlistSettingAction));

            SelectPlaylistCommand = ReactiveCommand.Create(() =>
            {
                _playlistSettingAction(_playlist);
            });
        }
    }

    public sealed class TrackViewModel : ViewModelBase
    {
        private readonly AggregatedTrack _trackState;

        public string TrackTitle => _trackState.Name;

        public TrackViewModel(AggregatedTrack trackState)
        {
            _trackState = trackState ?? throw new ArgumentNullException(nameof(trackState)); ;
        }
    }
}
