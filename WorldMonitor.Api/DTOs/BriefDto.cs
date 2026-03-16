namespace WorldMonitor.Api.DTOs;

public record BriefDto(
    int Id,
    string Title,
    string Content,
    string Model,
    int ArticleCount,
    DateTime GeneratedAt
);
