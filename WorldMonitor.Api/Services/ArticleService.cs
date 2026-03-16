using Microsoft.EntityFrameworkCore;
using WorldMonitor.Api.Data;
using WorldMonitor.Api.DTOs;

namespace WorldMonitor.Api.Services;

public class ArticleService : IArticleService
{
    private readonly AppDbContext _db;

    public ArticleService(AppDbContext db) => _db = db;

    public async Task<IEnumerable<ArticleDto>> GetRecentAsync(int count = 50, string? source = null, string? category = null)
    {
        var query = _db.Articles
            .Include(a => a.FeedSource)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(source))
            query = query.Where(a => a.FeedSource.Name.Contains(source));

        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(a => a.Category.Contains(category));

        return await query
            .OrderByDescending(a => a.PublishedAt)
            .Take(count)
            .Select(a => new ArticleDto(
                a.Id,
                a.Title,
                a.Summary,
                a.Url,
                a.ImageUrl,
                a.Author,
                a.Category,
                a.FeedSource.Name,
                a.PublishedAt))
            .ToListAsync();
    }

    public async Task<ArticleDto?> GetByIdAsync(int id)
    {
        return await _db.Articles
            .Include(a => a.FeedSource)
            .Where(a => a.Id == id)
            .Select(a => new ArticleDto(
                a.Id,
                a.Title,
                a.Summary,
                a.Url,
                a.ImageUrl,
                a.Author,
                a.Category,
                a.FeedSource.Name,
                a.PublishedAt))
            .FirstOrDefaultAsync();
    }
}
