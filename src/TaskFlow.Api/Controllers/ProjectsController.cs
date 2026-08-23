using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Interfaces;
using TaskFlow.Application.Projects;
using TaskFlow.Domain.Entities;
using TaskFlow.Infrastructure.Persistence;

namespace TaskFlow.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/projects")]
public class ProjectsController : ControllerBase
{
    private readonly TaskFlowDbContext _dbContext;
    private readonly ICurrentUserService _currentUser;

    public ProjectsController(
        TaskFlowDbContext dbContext,
        ICurrentUserService currentUser)
    {
        _dbContext = dbContext;
        _currentUser = currentUser;
    }

    [HttpPost]
    public async Task<ActionResult<ProjectResponse>> Create(
        CreateProjectRequest request)
    {
        var project = new Project
        {
            Name = request.Name.Trim(),
            Description = request.Description?.Trim(),
            UserId = _currentUser.UserId
        };

        _dbContext.Projects.Add(project);
        await _dbContext.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetById),
            new { id = project.Id },
            ToResponse(project));
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProjectResponse>>> GetAll()
    {
        var projects = await _dbContext.Projects
            .AsNoTracking()
            .Where(x => x.UserId == _currentUser.UserId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .Select(x => new ProjectResponse(
                x.Id,
                x.Name,
                x.Description,
                x.CreatedAtUtc,
                x.UpdatedAtUtc))
            .ToListAsync();

        return Ok(projects);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProjectResponse>> GetById(Guid id)
    {
        var project = await _dbContext.Projects
            .AsNoTracking()
            .SingleOrDefaultAsync(x =>
                x.Id == id &&
                x.UserId == _currentUser.UserId);

        if (project is null)
        {
            return NotFound();
        }

        return Ok(ToResponse(project));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ProjectResponse>> Update(
        Guid id,
        UpdateProjectRequest request)
    {
        var project = await _dbContext.Projects
            .SingleOrDefaultAsync(x =>
                x.Id == id &&
                x.UserId == _currentUser.UserId);

        if (project is null)
        {
            return NotFound();
        }

        project.Name = request.Name.Trim();
        project.Description = request.Description?.Trim();
        project.UpdatedAtUtc = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

        return Ok(ToResponse(project));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var project = await _dbContext.Projects
            .SingleOrDefaultAsync(x =>
                x.Id == id &&
                x.UserId == _currentUser.UserId);

        if (project is null)
        {
            return NotFound();
        }

        _dbContext.Projects.Remove(project);
        await _dbContext.SaveChangesAsync();

        return NoContent();
    }

    private static ProjectResponse ToResponse(Project project)
    {
        return new ProjectResponse(
            project.Id,
            project.Name,
            project.Description,
            project.CreatedAtUtc,
            project.UpdatedAtUtc);
    }
}
