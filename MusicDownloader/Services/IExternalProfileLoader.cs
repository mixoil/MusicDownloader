using MusicDownloader.Models.ExternalProfile;
using System.Threading.Tasks;

namespace MusicDownloader.Services
{
    public interface IExternalProfileLoader
    {
        public Task<ExternalProfile> LoadProfileAsync();
    }
}
