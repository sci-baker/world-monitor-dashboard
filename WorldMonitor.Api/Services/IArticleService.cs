using WorldMonitor.Api.DTOs;

namespace WorldMonitor.Api.Services;

public interface IArticleService
{
    Task<IEnumerable<ArticleDto>> GetRecentAsync(int count = 50, string? source = null, string? category = null);
    Task<ArticleDto?> GetByIdAsync(int id);
}
