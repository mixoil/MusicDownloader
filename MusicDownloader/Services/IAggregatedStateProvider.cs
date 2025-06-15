using MusicDownloader.Models.AggregatedProfile;
using MusicDownloader.Models.ExternalProfile;
using System.Threading.Tasks;

namespace MusicDownloader.Services
{
    public interface IAggregatedStateProvider
    {
        public Task<AggregatedProfile> LoadPersistedAsync();

        public Task<AggregatedProfile> MixPersistedWithExternalAsync(ExternalProfile externalProfile);
    }
}
