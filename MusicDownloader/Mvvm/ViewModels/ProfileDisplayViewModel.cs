using MusicDownloader.Mvvm.Infrastructure;
using MusicDownloader.Models;
using MusicDownloader.Services;
using System;
using ReactiveUI;
using System.Reactive;
using System.Threading.Tasks;

namespace MusicDownloader.Mvvm.ViewModels
{
    public sealed class ProfileDisplayViewModel : ViewModelBase
    {
        /// <summary>
        /// Initialize profile state command.
        /// </summary>
        public ReactiveCommand<Unit, Unit> InitProfileStateCommand { get; }

        /// <summary>
        /// Whether the folder is initialized.
        /// </summary>
        public bool IsMusicFolderInitialized => _profileStateProvider.IsProfileInitialized();

        public ProfileState? ProfileState => _profileState;

        private ProfileState? _profileState;
        private readonly IProfileStateProvider _profileStateProvider;

        public ProfileDisplayViewModel(IProfileStateProvider profileStateProvider)
        {
            _profileStateProvider = profileStateProvider ?? throw new ArgumentNullException(nameof(profileStateProvider));

            InitProfileStateCommand = ReactiveCommand.Create(() =>
            {
                _profileState = _profileStateProvider.CreateProfileState();
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
                });
            }
        }
    }
}