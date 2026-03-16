using Microsoft.EntityFrameworkCore;
using WorldMonitor.Api.Data;
using WorldMonitor.Api.Entities;

namespace WorldMonitor.Api.Infrastructure;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        if (await db.MapEvents.AnyAsync())
            return;

        var now = DateTime.UtcNow;

        db.MapEvents.AddRange(
            new MapEvent
            {
                Title = "Strikes reported in Tehran",
                Description = "Emergency response activity reported after overnight strikes.",
                Latitude = 35.6892,
                Longitude = 51.3890,
                Category = "conflict",
                Severity = "critical",
                SourceUrl = "https://www.aljazeera.com/",
                EventDate = now.AddHours(-2)
            },
            new MapEvent
            {
                Title = "Humanitarian incident in Gaza",
                Description = "Civilian casualties reported by multiple outlets.",
                Latitude = 31.5017,
                Longitude = 34.4668,
                Category = "humanitarian",
                Severity = "high",
                SourceUrl = "https://www.bbc.com/",
                EventDate = now.AddHours(-4)
            },
            new MapEvent
            {
                Title = "Security incident in Kabul",
                Description = "Regional security update and local response activity.",
                Latitude = 34.5553,
                Longitude = 69.2075,
                Category = "security",
                Severity = "medium",
                SourceUrl = "https://www.aljazeera.com/",
                EventDate = now.AddHours(-6)
            },
            new MapEvent
            {
                Title = "Shipping risk near Strait of Hormuz",
                Description = "Maritime traffic concerns affecting regional logistics.",
                Latitude = 26.5667,
                Longitude = 56.2500,
                Category = "trade",
                Severity = "high",
                SourceUrl = "https://www.bbc.com/",
                EventDate = now.AddHours(-8)
            },
            new MapEvent
            {
                Title = "Cross-border incident in Khost",
                Description = "Local authorities reported casualties after an attack.",
                Latitude = 33.3395,
                Longitude = 69.9204,
                Category = "conflict",
                Severity = "medium",
                SourceUrl = "https://www.aljazeera.com/",
                EventDate = now.AddHours(-10)
            },
            new MapEvent
            {
                Title = "Agricultural disruption in northern India",
                Description = "Crop protection efforts reported in rural districts.",
                Latitude = 30.9000,
                Longitude = 75.8500,
                Category = "society",
                Severity = "low",
                SourceUrl = "https://www.aljazeera.com/",
                EventDate = now.AddHours(-12)
            }
        );

        await db.SaveChangesAsync();
    }
}