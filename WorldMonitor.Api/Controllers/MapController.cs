using Microsoft.AspNetCore.Mvc;
using WorldMonitor.Api.DTOs;
using WorldMonitor.Api.Services;

namespace WorldMonitor.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MapController : ControllerBase
{
    private readonly IMapEventService _mapService;

    public MapController(IMapEventService mapService) => _mapService = mapService;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? category = null)
    {
        var events = await _mapService.GetAllAsync(category);
        return Ok(events);
    }

    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories()
    {
        var categories = await _mapService.GetCategoriesAsync();
        return Ok(categories);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var ev = await _mapService.GetByIdAsync(id);
        if (ev is null) return NotFound(new { error = "Map event not found" });
        return Ok(ev);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMapEventDto dto)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var created = await _mapService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }
}