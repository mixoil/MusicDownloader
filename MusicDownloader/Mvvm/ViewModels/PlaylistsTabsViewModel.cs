using MusicDownloader.Models;
using MusicDownloader.Mvvm.Infrastructure;
using MusicDownloader.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MusicDownloader.Mvvm.ViewModels
{
    public sealed class PlaylistsTabsViewModel : ViewModelBase
    {
        private readonly IProfileStateProvider _profileStateProvider;

        private ProfileState? _profileState;

        public ProfileState? ProfileState => _profileState;

        public List<PlaylistState>? Playlists => _profileState?.PlaylistStates;

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
                });
            }
        }
    }
}
