namespace WorldMonitor.Api.Entities;

public class MapEvent
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string Category { get; set; } = string.Empty;    // e.g. "conflict", "disaster", "politics"
    public string Severity { get; set; } = "medium";        // low, medium, high, critical
    public string? SourceUrl { get; set; }
    public DateTime EventDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
