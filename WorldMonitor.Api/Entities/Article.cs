namespace WorldMonitor.Api.Entities;

public class Article
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string UrlHash { get; set; } = string.Empty;   // SHA-256 for dedup
    public string? ImageUrl { get; set; }
    public string? Author { get; set; }
    public string Category { get; set; } = string.Empty;
    public DateTime PublishedAt { get; set; }
    public DateTime IngestedAt { get; set; } = DateTime.UtcNow;

    // FK
    public int FeedSourceId { get; set; }
    public FeedSource FeedSource { get; set; } = null!;
}
