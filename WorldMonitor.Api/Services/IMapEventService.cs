using WorldMonitor.Api.DTOs;

namespace WorldMonitor.Api.Services;

public interface IMapEventService
{
    Task<IReadOnlyList<MapEventDto>> GetAllAsync(string? category = null);
    Task<MapEventDto?> GetByIdAsync(int id);
    Task<MapEventDto> CreateAsync(CreateMapEventDto dto);
    Task<IReadOnlyList<string>> GetCategoriesAsync();
}