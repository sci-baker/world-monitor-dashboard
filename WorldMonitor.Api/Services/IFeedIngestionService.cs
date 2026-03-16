namespace WorldMonitor.Api.Services;

public interface IFeedIngestionService
{
    /// <summary>
    /// Fetch all active feeds, parse articles, deduplicate, and persist new ones.
    /// Returns the number of newly ingested articles.
    /// </summary>
    Task<int> IngestAllFeedsAsync();
}
