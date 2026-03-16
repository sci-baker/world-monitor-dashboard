using Microsoft.AspNetCore.Mvc;
using WorldMonitor.Api.Services;

namespace WorldMonitor.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BriefController : ControllerBase
{
    private readonly IAiBriefService _briefService;

    public BriefController(IAiBriefService briefService) => _briefService = briefService;

    /// <summary>
    /// Get the most recent AI-generated intelligence brief.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetLatest()
    {
        var brief = await _briefService.GetLatestBriefAsync();
        if (brief is null) return NotFound(new { error = "No briefs generated yet. Call POST /api/brief/generate first." });
        return Ok(brief);
    }

    /// <summary>
    /// Generate a new AI intelligence brief from recent articles via Ollama/Qwen.
    /// </summary>
    [HttpPost("generate")]
    public async Task<IActionResult> Generate()
    {
        var brief = await _briefService.GenerateBriefAsync();
        return Ok(brief);
    }
}
