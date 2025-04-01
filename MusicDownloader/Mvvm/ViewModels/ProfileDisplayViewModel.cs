using MusicDownloader.Mvvm.Infrastructure;
using MusicDownloader.Models;
using MusicDownloader.Services;
using System;
using ReactiveUI;
using System.Reactive;

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

        private readonly IProfileStateProvider _profileStateProvider;

        public ProfileDisplayViewModel(IProfileStateProvider profileStateProvider)
        {
            _profileStateProvider = profileStateProvider ?? throw new ArgumentNullException(nameof(profileStateProvider));

            InitProfileStateCommand = ReactiveCommand.Create(() =>
            {
                _ = _profileStateProvider.CreateProfileState();
                OnPropertyChanged(nameof(IsMusicFolderInitialized));
            });
        }
    }
}