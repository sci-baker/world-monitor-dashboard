using Microsoft.EntityFrameworkCore;
using WorldMonitor.Api.Data;
using WorldMonitor.Api.DTOs;
using WorldMonitor.Api.Entities;

namespace WorldMonitor.Api.Services;

public class MapEventService : IMapEventService
{
    private readonly AppDbContext _db;

    public MapEventService(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<MapEventDto>> GetAllAsync(string? category = null)
    {
        var query = _db.MapEvents
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(category))
        {
            var normalized = category.Trim().ToLower();
            query = query.Where(e => e.Category != null && e.Category.ToLower() == normalized);
        }

        return await query
            .OrderByDescending(e => e.EventDate)
            .Select(e => new MapEventDto(
                e.Id,
                e.Title,
                e.Description,
                e.Latitude,
                e.Longitude,
                e.Category,
                e.Severity,
                e.SourceUrl,
                e.EventDate
            ))
            .ToListAsync();
    }

    public async Task<MapEventDto?> GetByIdAsync(int id)
    {
        var entity = await _db.MapEvents
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id);

        if (entity is null) return null;

        return new MapEventDto(
            entity.Id,
            entity.Title,
            entity.Description,
            entity.Latitude,
            entity.Longitude,
            entity.Category,
            entity.Severity,
            entity.SourceUrl,
            entity.EventDate
        );
    }

    public async Task<MapEventDto> CreateAsync(CreateMapEventDto dto)
    {
        var entity = new MapEvent
        {
            Title = dto.Title.Trim(),
            Description = dto.Description?.Trim(),
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            Category = dto.Category.Trim().ToLowerInvariant(),
            Severity = string.IsNullOrWhiteSpace(dto.Severity)
                ? "medium"
                : dto.Severity.Trim().ToLowerInvariant(),
            SourceUrl = dto.SourceUrl?.Trim(),
            EventDate = dto.EventDate?.ToUniversalTime() ?? DateTime.UtcNow
        };

        _db.MapEvents.Add(entity);
        await _db.SaveChangesAsync();

        return new MapEventDto(
            entity.Id,
            entity.Title,
            entity.Description,
            entity.Latitude,
            entity.Longitude,
            entity.Category,
            entity.Severity,
            entity.SourceUrl,
            entity.EventDate
        );
    }

    public async Task<IReadOnlyList<string>> GetCategoriesAsync()
    {
        return await _db.MapEvents
            .AsNoTracking()
            .Where(e => e.Category != null && e.Category != "")
            .Select(e => e.Category!)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync();
    }
}