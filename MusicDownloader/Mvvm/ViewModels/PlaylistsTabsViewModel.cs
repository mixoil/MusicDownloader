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
    public sealed class PlaylistsTabsViewModel : ViewModelBase
    {
        private readonly IProfileStateProvider _profileStateProvider;

        private ProfileState? _profileState;

        private PlaylistState? _currentPlaylist;

        public ProfileState? ProfileState => _profileState;

        public List<PlaylistBtnViewModel>? PlaylistBtns => _profileState?.PlaylistStates
            .Select(p => new PlaylistBtnViewModel(p, SelectPlaylist))
            .ToList();

        public string? CurrentPlaylist => _currentPlaylist?.Title;

        /// <summary>
        /// Constructor for designer.
        /// </summary>
#pragma warning disable CS8618
        public PlaylistsTabsViewModel() { }
#pragma warning restore CS8618

        public PlaylistsTabsViewModel(IProfileStateProvider profileStateProvider)
        {
            _profileStateProvider = profileStateProvider ?? throw new ArgumentNullException(nameof(profileStateProvider));

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
}
