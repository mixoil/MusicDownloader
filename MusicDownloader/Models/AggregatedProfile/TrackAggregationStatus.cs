
namespace MusicDownloader.Models.AggregatedProfile
{
    //Revork - must be two statuses
    public enum TrackAggregationStatus
    {
        PersistedAndExternalStatusUnknown,
        PersistedAndPresentedInExternal,
        PersistedAndNotPresentedInExternal,
        NotPersistedAndPresentedInExternal,
    }
}
