namespace WorldMonitor.Api.DTOs;

public record MapEventDto(
    int Id,
    string Title,
    string Description,
    double Latitude,
    double Longitude,
    string Category,
    string Severity,
    string? SourceUrl,
    DateTime EventDate
);

public record CreateMapEventDto(
    string Title,
    string Description,
    double Latitude,
    double Longitude,
    string Category,
    string Severity = "medium",
    string? SourceUrl = null,
    DateTime? EventDate = null
);
