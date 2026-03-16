using Microsoft.EntityFrameworkCore;
using WorldMonitor.Api.Entities;

namespace WorldMonitor.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<FeedSource> FeedSources => Set<FeedSource>();
    public DbSet<Article> Articles => Set<Article>();
    public DbSet<Brief> Briefs => Set<Brief>();
    public DbSet<MapEvent> MapEvents => Set<MapEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ---------- FeedSource ----------
        modelBuilder.Entity<FeedSource>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
            e.Property(x => x.Url).HasMaxLength(500).IsRequired();
            e.Property(x => x.Category).HasMaxLength(100);
        });

        // ---------- Article ----------
        modelBuilder.Entity<Article>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Title).HasMaxLength(500).IsRequired();
            e.Property(x => x.Url).HasMaxLength(1000).IsRequired();
            e.Property(x => x.UrlHash).HasMaxLength(64).IsRequired();
            e.Property(x => x.Category).HasMaxLength(100);

            e.HasIndex(x => x.UrlHash).IsUnique();          // dedup index
            e.HasIndex(x => x.PublishedAt);

            e.HasOne(x => x.FeedSource)
             .WithMany(f => f.Articles)
             .HasForeignKey(x => x.FeedSourceId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // ---------- Brief ----------
        modelBuilder.Entity<Brief>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Title).HasMaxLength(300);
            e.Property(x => x.Content).IsRequired();
            e.Property(x => x.Model).HasMaxLength(100);
        });

        // ---------- MapEvent ----------
        modelBuilder.Entity<MapEvent>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Title).HasMaxLength(300).IsRequired();
            e.Property(x => x.Category).HasMaxLength(100);
            e.Property(x => x.Severity).HasMaxLength(20);
            e.Property(x => x.SourceUrl).HasMaxLength(1000);

            e.HasIndex(x => x.EventDate);
        });

        // ---------- Seed default feed sources ----------
        modelBuilder.Entity<FeedSource>().HasData(
            new FeedSource { Id = 1, Name = "Reuters World", Url = "https://feeds.reuters.com/reuters/worldNews", Category = "world" },
            new FeedSource { Id = 2, Name = "BBC World", Url = "https://feeds.bbci.co.uk/news/world/rss.xml", Category = "world" },
            new FeedSource { Id = 3, Name = "Al Jazeera", Url = "https://www.aljazeera.com/xml/rss/all.xml", Category = "world" }
        );
    }
}
