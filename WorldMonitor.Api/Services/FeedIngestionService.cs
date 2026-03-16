using System.Security.Cryptography;
using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;
using Microsoft.EntityFrameworkCore;
using WorldMonitor.Api.Data;
using WorldMonitor.Api.Entities;

namespace WorldMonitor.Api.Services;

public class FeedIngestionService : IFeedIngestionService
{
    private readonly AppDbContext _db;
    private readonly HttpClient _http;
    private readonly ILogger<FeedIngestionService> _log;

    public FeedIngestionService(AppDbContext db, HttpClient http, ILogger<FeedIngestionService> log)
    {
        _db = db;
        _http = http;
        _log = log;
    }

    public async Task<int> IngestAllFeedsAsync()
    {
        var sources = await _db.FeedSources.Where(s => s.IsActive).ToListAsync();
        int totalNew = 0;

        foreach (var source in sources)
        {
            try
            {
                var count = await IngestFeedAsync(source);
                totalNew += count;
                _log.LogInformation("Ingested {Count} new articles from {Source}", count, source.Name);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Failed to ingest feed: {Source} ({Url})", source.Name, source.Url);
            }
        }

        return totalNew;
    }

    private async Task<int> IngestFeedAsync(FeedSource source)
    {
        var stream = await _http.GetStreamAsync(source.Url);
        using var reader = XmlReader.Create(stream);
        var feed = SyndicationFeed.Load(reader);

        if (feed == null) return 0;

        int newCount = 0;

        foreach (var item in feed.Items)
        {
            var link = item.Links.FirstOrDefault()?.Uri?.AbsoluteUri;
            if (string.IsNullOrWhiteSpace(link)) continue;

            var hash = ComputeHash(link);

            // Dedup check
            bool exists = await _db.Articles.AnyAsync(a => a.UrlHash == hash);
            if (exists) continue;

            var article = new Article
            {
                Title = Truncate(item.Title?.Text ?? "Untitled", 500),
                Summary = Truncate(item.Summary?.Text ?? string.Empty, 2000),
                Url = link,
                UrlHash = hash,
                ImageUrl = ExtractImageUrl(item),
                Author = item.Authors.FirstOrDefault()?.Name,
                Category = source.Category,
                PublishedAt = item.PublishDate != default
                    ? item.PublishDate.UtcDateTime
                    : DateTime.UtcNow,
                FeedSourceId = source.Id
            };

            _db.Articles.Add(article);
            newCount++;
        }

        if (newCount > 0)
        {
            source.LastFetchedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }

        return newCount;
    }

    // --- Helpers ---

    private static string ComputeHash(string input)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexStringLower(bytes);
    }

    private static string? ExtractImageUrl(SyndicationItem item)
    {
        // Try to find an image link (enclosure or media:content)
        var enclosure = item.Links.FirstOrDefault(l =>
            l.MediaType?.StartsWith("image", StringComparison.OrdinalIgnoreCase) == true);

        return enclosure?.Uri?.AbsoluteUri;
    }

    private static string Truncate(string value, int maxLength) =>
        value.Length <= maxLength ? value : value[..maxLength];
}
