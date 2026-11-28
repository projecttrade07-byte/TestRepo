using System.ComponentModel.DataAnnotations;
using EventApp.Data;
using EventApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventApp.Controllers;

[ApiController]
[Route("api/events")]
public sealed class EventsController(EventDbContext database) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EventResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var events = await database.Events
            .AsNoTracking()
            .OrderBy(item => item.StartsAt)
            .Select(item => new EventResponse(
                item.Id,
                item.Title,
                item.Location,
                item.Description,
                item.StartsAt,
                item.EndsAt))
            .ToListAsync(cancellationToken);

        return Ok(events);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<EventResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var item = await database.Events
            .AsNoTracking()
            .Where(eventItem => eventItem.Id == id)
            .Select(eventItem => new EventResponse(
                eventItem.Id,
                eventItem.Title,
                eventItem.Location,
                eventItem.Description,
                eventItem.StartsAt,
                eventItem.EndsAt))
            .SingleOrDefaultAsync(cancellationToken);

        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<EventResponse>> Create(
        CreateEventRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            ModelState.AddModelError(nameof(request.Title), "Event name is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Location))
        {
            ModelState.AddModelError(nameof(request.Location), "Location is required.");
        }

        if (request.EndsAt <= request.StartsAt)
        {
            ModelState.AddModelError(nameof(request.EndsAt), "End time must be after the start time.");
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var item = new Event
        {
            Title = request.Title.Trim(),
            Location = request.Location.Trim(),
            Description = string.IsNullOrWhiteSpace(request.Description)
                ? null
                : request.Description.Trim(),
            StartsAt = request.StartsAt,
            EndsAt = request.EndsAt
        };

        database.Events.Add(item);
        await database.SaveChangesAsync(cancellationToken);

        var response = new EventResponse(
            item.Id,
            item.Title,
            item.Location,
            item.Description,
            item.StartsAt,
            item.EndsAt);

        return CreatedAtAction(nameof(GetById), new { id = item.Id }, response);
    }
}

public sealed class CreateEventRequest
{
    [Required]
    [StringLength(120, MinimumLength = 1)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string Location { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    public DateTime StartsAt { get; set; }

    public DateTime EndsAt { get; set; }
}

public sealed record EventResponse(
    int Id,
    string Title,
    string Location,
    string? Description,
    DateTime StartsAt,
    DateTime EndsAt);
