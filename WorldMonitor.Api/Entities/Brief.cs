namespace WorldMonitor.Api.Entities;

public class Brief
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;       
    public int ArticleCount { get; set; }                    // how many articles were summarised
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
}
