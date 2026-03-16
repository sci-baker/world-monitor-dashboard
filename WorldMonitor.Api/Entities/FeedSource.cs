namespace WorldMonitor.Api.Entities;

public class FeedSource
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime LastFetchedAt { get; set; }

    // Navigation
    public ICollection<Article> Articles { get; set; } = new List<Article>();
}
