using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WorldMonitor.Api.Data;
using WorldMonitor.Api.DTOs;
using WorldMonitor.Api.Entities;

namespace WorldMonitor.Api.Services;

// --- Options class matching appsettings ---
public class OllamaOptions
{
    public const string Section = "Ollama";
    public string BaseUrl { get; set; } = "http://localhost:11434";
    public string Model { get; set; } = "qwen2:7b";
}

public class AiBriefService : IAiBriefService
{
    private readonly AppDbContext _db;
    private readonly HttpClient _http;
    private readonly OllamaOptions _opts;
    private readonly ILogger<AiBriefService> _log;

    public AiBriefService(
        AppDbContext db,
        HttpClient http,
        IOptions<OllamaOptions> opts,
        ILogger<AiBriefService> log)
    {
        _db = db;
        _http = http;
        _opts = opts.Value;
        _log = log;
    }

    public async Task<BriefDto> GenerateBriefAsync()
    {
        // 1. Gather recent articles (last 24 h, up to 30)
        var since = DateTime.UtcNow.AddHours(-24);
        var articles = await _db.Articles
            .Where(a => a.PublishedAt >= since)
            .OrderByDescending(a => a.PublishedAt)
            .Take(30)
            .ToListAsync();

        if (articles.Count == 0)
        {
            // Try last 7 days if no recent articles
            since = DateTime.UtcNow.AddDays(-7);
            articles = await _db.Articles
                .OrderByDescending(a => a.PublishedAt>=since)
                .Take(30)
                .ToListAsync();
        }

        // 2. Build the prompt
        var sb = new StringBuilder();
        sb.AppendLine("You are an intelligence analyst. Given the following news headlines and summaries, generate a concise daily intelligence brief.");
        sb.AppendLine("Structure it with: Executive Summary, Key Developments (bullet points), and Risk Assessment.");
        sb.AppendLine("Be factual, concise, and professional.\n");
        sb.AppendLine("--- ARTICLES ---");

        foreach (var a in articles)
        {
            sb.AppendLine($"• [{a.Category}] {a.Title}");
            if (!string.IsNullOrWhiteSpace(a.Summary))
            {
                var summary = a.Summary.Length > 300 ? a.Summary[..300] + "…" : a.Summary;
                sb.AppendLine($"  {summary}");
            }
        }

        sb.AppendLine("\n--- END ARTICLES ---");
        sb.AppendLine("\nGenerate the intelligence brief now:");

        var prompt = sb.ToString();

        // 3. Call Ollama API
        var content = await CallOllamaAsync(prompt);

        // 4. Save the brief
        var brief = new Brief
        {
            Title = $"Intelligence Brief — {DateTime.UtcNow:yyyy-MM-dd HH:mm} UTC",
            Content = content,
            Model = _opts.Model,
            ArticleCount = articles.Count
        };

        _db.Briefs.Add(brief);
        await _db.SaveChangesAsync();

        return ToDto(brief);
    }

    public async Task<BriefDto?> GetLatestBriefAsync()
    {
        var brief = await _db.Briefs
            .OrderByDescending(b => b.GeneratedAt)
            .FirstOrDefaultAsync();

        return brief is null ? null : ToDto(brief);
    }

    // ---- Ollama HTTP call ----

    private async Task<string> CallOllamaAsync(string prompt)
    {
        var requestBody = new
        {
            model = _opts.Model,
            prompt,
            stream = false
        };

        var json = JsonSerializer.Serialize(requestBody);
        var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

        var url = $"{_opts.BaseUrl.TrimEnd('/')}/api/generate";
        _log.LogInformation("Calling Ollama at {Url} with model {Model}", url, _opts.Model);

        var response = await _http.PostAsync(url, httpContent);
        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync();

        // Ollama response: { "model": "...", "response": "...", ... }
        using var doc = JsonDocument.Parse(responseJson);
        var result = doc.RootElement.GetProperty("response").GetString()
            ?? "Failed to extract response from Ollama.";

        return result;
    }

    private static BriefDto ToDto(Brief b) =>
        new(b.Id, b.Title, b.Content, b.Model, b.ArticleCount, b.GeneratedAt);
}
