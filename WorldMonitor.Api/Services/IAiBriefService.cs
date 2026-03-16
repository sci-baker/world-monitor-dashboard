using WorldMonitor.Api.DTOs;

namespace WorldMonitor.Api.Services;

public interface IAiBriefService
{
    Task<BriefDto> GenerateBriefAsync();
    Task<BriefDto?> GetLatestBriefAsync();
}
