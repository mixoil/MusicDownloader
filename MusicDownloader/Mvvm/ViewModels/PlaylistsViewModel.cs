using MusicDownloader.Models;
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
        /// Whether the folder is initialized.
        /// </summary>
        public bool IsMusicFolderInitialized => _profileStateProvider.IsProfileInitialized();

        private readonly IProfileStateProvider _profileStateProvider;

        private ProfileState? _profileState;

        private PlaylistState? _currentPlaylist;

        public ProfileState? ProfileState => _profileState;

        public List<PlaylistBtnViewModel>? PlaylistBtns => _profileState?.PlaylistStates
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

        public PlaylistsViewModel(IProfileStateProvider profileStateProvider)
        {
            _profileStateProvider = profileStateProvider ?? throw new ArgumentNullException(nameof(profileStateProvider));

            InitProfileStateCommand = ReactiveCommand.Create(() =>
            {
                _ = _profileStateProvider.CreateProfileState();
                OnPropertyChanged(nameof(IsMusicFolderInitialized));
            });

            LoadProfile();
        }

        private void LoadProfile()
        {
            if (_profileStateProvider.IsProfileInitialized())
            {
                Task.Run(async () =>
                {
                    _profileState = await _profileStateProvider.LoadProfileStateAsync();
                    OnPropertyChanged(nameof(PlaylistBtns));
                });
            }
        }

        private void SelectPlaylist(PlaylistState playlist)
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

        private readonly Action<PlaylistState> _playlistSettingAction;
        private readonly PlaylistState _playlist;

        public PlaylistBtnViewModel(PlaylistState playlist, Action<PlaylistState> playlistSettingAction)
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
        private readonly TrackState _trackState;

        public string TrackTitle => _trackState.Name;

        public TrackViewModel(TrackState trackState)
        {
            _trackState = trackState ?? throw new ArgumentNullException(nameof(trackState)); ;
        }
    }
}
