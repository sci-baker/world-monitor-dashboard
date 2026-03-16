using Microsoft.AspNetCore.Mvc;
using WorldMonitor.Api.Services;

namespace WorldMonitor.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NewsController : ControllerBase
{
    private readonly IArticleService _articleService;
    private readonly IFeedIngestionService _feedService;

    public NewsController(IArticleService articleService, IFeedIngestionService feedService)
    {
        _articleService = articleService;
        _feedService = feedService;
    }

    /// <summary>
    /// Get recent articles, optionally filtered by source and/or category.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetArticles(
        [FromQuery] int count = 50,
        [FromQuery] string? source = null,
        [FromQuery] string? category = null)
    {
        var articles = await _articleService.GetRecentAsync(count, source, category);
        return Ok(articles);
    }

    /// <summary>
    /// Get a single article by ID.
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetArticle(int id)
    {
        var article = await _articleService.GetByIdAsync(id);
        if (article is null) return NotFound(new { error = "Article not found" });
        return Ok(article);
    }

    /// <summary>
    /// Trigger RSS feed ingestion from all active sources.
    /// </summary>
    [HttpPost("ingest")]
    public async Task<IActionResult> Ingest()
    {
        var newCount = await _feedService.IngestAllFeedsAsync();
        return Ok(new { message = $"Ingestion complete. {newCount} new article(s) added." });
    }
}
