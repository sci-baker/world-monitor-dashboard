using Microsoft.EntityFrameworkCore;
using WorldMonitor.Api.Data;
using WorldMonitor.Api.Infrastructure;
using WorldMonitor.Api.Middleware;
using WorldMonitor.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// ---------- Database ----------
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// ---------- Ollama Options ----------
builder.Services.Configure<OllamaOptions>(
    builder.Configuration.GetSection(OllamaOptions.Section));

// ---------- HttpClient ----------
builder.Services.AddHttpClient<IFeedIngestionService, FeedIngestionService>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
    client.DefaultRequestHeaders.UserAgent.ParseAdd("WorldMonitor/1.0");
});

builder.Services.AddHttpClient<IAiBriefService, AiBriefService>(client =>
{
    client.Timeout = TimeSpan.FromMinutes(5);
});

// ---------- Services ----------
builder.Services.AddScoped<IArticleService, ArticleService>();
builder.Services.AddScoped<IMapEventService, MapEventService>();

// ---------- Controllers + Swagger ----------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ---------- CORS ----------
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// ---------- Auto-migrate + seed on startup ----------
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
    await DbSeeder.SeedAsync(db);
}

// ---------- Middleware Pipeline ----------
app.UseMiddleware<ErrorHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.MapControllers();

app.Run();