using MusicDownloader.Models;
using System;
using System.Threading.Tasks;

namespace MusicDownloader.Services
{
    /// <inheritdoc cref="IProfileStateProvider"/>
    public class ProfileStateProvider : IProfileStateProvider
    {
        /// <inheritdoc/>
        public async Task<ProfileState> LoadOrCreateProfileStateAsync()
        {
            throw new NotImplementedException();
        }
    }
}
