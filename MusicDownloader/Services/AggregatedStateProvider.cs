using MusicDownloader.Models.AggregatedProfile;
using MusicDownloader.Models.ExternalProfile;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MusicDownloader.Services
{
    public sealed class AggregatedStateProvider : IAggregatedStateProvider
    {
        private readonly IPersistedProfileProvider _persistedProfileProvider;

        public AggregatedStateProvider(IPersistedProfileProvider persistedProfileProvider)
        {
            _persistedProfileProvider = persistedProfileProvider ?? throw new ArgumentNullException(nameof(persistedProfileProvider));
        }

        public async Task<AggregatedProfile> LoadPersistedAsync()
        {
            var profileState = await _persistedProfileProvider.LoadProfileStateAsync();

            return new AggregatedProfile
            {
                //TODO: move to automapper
                Playlists = profileState.PlaylistStates.Select(p => new AggregatedPlaylist
                {
                    Title = p.Title,
                    Tracks = p.Tracks.Select(t => new AggregatedTrack
                    {
                        Album = t.Album,
                        Author = t.Author,
                        Duration = t.Duration,
                        InternalAlbumId = t.InternalAlbumId,
                        InternalTrackId = t.InternalTrackId,
                        Name = t.Name,
                        OriginalAlbum = t.OriginalAlbum,
                        OriginalAuthor = t.OriginalAuthor,
                        OriginalDuration = t.OriginalDuration,
                        OriginalName = t.OriginalName,
                        Status = TrackAggregationStatus.PersistedAndExternalStatusUnknown
                    }).ToArray()
                }).ToArray()
            };
        }

        public async Task<AggregatedProfile> MixPersistedWithExternalAsync(ExternalProfile externalProfile)
        {
            var aggregatedProfileFromPersisted = await LoadPersistedAsync();

            var resPlaylists = aggregatedProfileFromPersisted.Playlists.ToList();

            foreach (var externalPlaylist in externalProfile.Playlists)
            {
                var persistedPlaylist = aggregatedProfileFromPersisted.Playlists
                    .FirstOrDefault(p => p.ExternalId == externalPlaylist.PlaylistId);

                if (persistedPlaylist == null)
                {
                    resPlaylists.Add(new AggregatedPlaylist
                    {
                        ExternalId = externalPlaylist.PlaylistId,
                        Title = externalPlaylist.Title,
                        Tracks = externalPlaylist.Tracks.Select(t => new AggregatedTrack
                        {
                            Album = t.Album,
                            Author = t.Author,
                            Duration = t.Duration,
                            Name = t.Name,
                            Status = TrackAggregationStatus.NotPersistedAndPresentedInExternal
                        }).ToArray()
                    });
                }
                else
                {
                    var persistedTracks = persistedPlaylist.Tracks.ToList();

                    foreach (var externalTrack in externalPlaylist.Tracks)
                    {
                        var persistedTrack = persistedPlaylist.Tracks
                            .FirstOrDefault(p => p.Name == externalTrack.Name
                            && p.Album == externalTrack.Album
                            && p.Author == externalTrack.Author
                            && p.Duration == externalTrack.Duration
                            );

                        if (persistedTrack == null)
                        {
                            persistedTracks.Add(new AggregatedTrack
                            {
                                Album = externalTrack.Album,
                                Author = externalTrack.Author,
                                Duration = externalTrack.Duration,
                                Name = externalTrack.Name,
                                Status = TrackAggregationStatus.NotPersistedAndPresentedInExternal
                            });
                        }
                        else
                        {
                            persistedTrack.Status = TrackAggregationStatus.PersistedAndPresentedInExternal;
                        }
                    }

                    persistedPlaylist.Tracks = persistedTracks;
                }

            }

            aggregatedProfileFromPersisted.Playlists = resPlaylists;

            return aggregatedProfileFromPersisted;
        }
    }
}
