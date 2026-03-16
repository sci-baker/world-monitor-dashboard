namespace WorldMonitor.Api.DTOs;

public record ArticleDto(
    int Id,
    string Title,
    string Summary,
    string Url,
    string? ImageUrl,
    string? Author,
    string Category,
    string SourceName,
    DateTime PublishedAt
);
