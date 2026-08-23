using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Interfaces;
using TaskFlow.Application.Tasks;
using TaskFlow.Domain.Entities;
using TaskFlow.Infrastructure.Persistence;

namespace TaskFlow.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/tasks")]
public class TasksController : ControllerBase
{
    private readonly TaskFlowDbContext _dbContext;
    private readonly ICurrentUserService _currentUser;

    public TasksController(
        TaskFlowDbContext dbContext,
        ICurrentUserService currentUser)
    {
        _dbContext = dbContext;
        _currentUser = currentUser;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TaskResponse>> GetById(Guid id)
    {
        var task = await GetOwnedTaskQuery()
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == id);

        if (task is null)
        {
            return NotFound();
        }

        return Ok(ToResponse(task));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TaskResponse>> Update(
        Guid id,
        UpdateTaskRequest request)
    {
        var task = await GetOwnedTaskQuery()
            .SingleOrDefaultAsync(x => x.Id == id);

        if (task is null)
        {
            return NotFound();
        }

        task.Title = request.Title.Trim();
        task.Description = request.Description?.Trim();
        task.Status = request.Status;
        task.Priority = request.Priority;
        task.DueDateUtc = request.DueDateUtc;
        task.UpdatedAtUtc = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

        return Ok(ToResponse(task));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var task = await GetOwnedTaskQuery()
            .SingleOrDefaultAsync(x => x.Id == id);

        if (task is null)
        {
            return NotFound();
        }

        _dbContext.TaskItems.Remove(task);
        await _dbContext.SaveChangesAsync();

        return NoContent();
    }

    private IQueryable<TaskItem> GetOwnedTaskQuery()
    {
        return _dbContext.TaskItems
            .Where(task =>
                task.Project.UserId == _currentUser.UserId);
    }

    private static TaskResponse ToResponse(TaskItem task)
    {
        return new TaskResponse(
            task.Id,
            task.ProjectId,
            task.Title,
            task.Description,
            task.Status,
            task.Priority,
            task.DueDateUtc,
            task.CreatedAtUtc,
            task.UpdatedAtUtc);
    }
}
