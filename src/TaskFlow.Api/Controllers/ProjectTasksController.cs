using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Interfaces;
using TaskFlow.Application.Tasks;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Enums;
using TaskFlow.Infrastructure.Persistence;

namespace TaskFlow.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/projects/{projectId:guid}/tasks")]
public class ProjectTasksController : ControllerBase
{
    private readonly TaskFlowDbContext _dbContext;
    private readonly ICurrentUserService _currentUser;

    public ProjectTasksController(
        TaskFlowDbContext dbContext,
        ICurrentUserService currentUser)
    {
        _dbContext = dbContext;
        _currentUser = currentUser;
    }

    [HttpPost]
    public async Task<ActionResult<TaskResponse>> Create(
        Guid projectId,
        CreateTaskRequest request)
    {
        var projectExists = await _dbContext.Projects
            .AnyAsync(x =>
                x.Id == projectId &&
                x.UserId == _currentUser.UserId);

        if (!projectExists)
        {
            return NotFound();
        }

        var task = new TaskItem
        {
            ProjectId = projectId,
            Title = request.Title.Trim(),
            Description = request.Description?.Trim(),
            Priority = request.Priority,
            DueDateUtc = request.DueDateUtc,
            Status = TaskItemStatus.Todo
        };

        _dbContext.TaskItems.Add(task);
        await _dbContext.SaveChangesAsync();

        return Created(
            $"/api/tasks/{task.Id}",
            ToResponse(task));
    }

    [HttpGet]
    public async Task<ActionResult<PagedResponse<TaskResponse>>> GetAll(
        Guid projectId,
        [FromQuery] TaskQueryParameters query)
    {
        var projectExists = await _dbContext.Projects
            .AnyAsync(x =>
                x.Id == projectId &&
                x.UserId == _currentUser.UserId);

        if (!projectExists)
        {
            return NotFound();
        }

        var tasksQuery = _dbContext.TaskItems
            .AsNoTracking()
            .Where(x => x.ProjectId == projectId);

        if (query.Status.HasValue)
        {
            tasksQuery = tasksQuery.Where(
                x => x.Status == query.Status.Value);
        }

        if (query.Priority.HasValue)
        {
            tasksQuery = tasksQuery.Where(
                x => x.Priority == query.Priority.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim();

            tasksQuery = tasksQuery.Where(x =>
                x.Title.Contains(search) ||
                (x.Description != null &&
                 x.Description.Contains(search)));
        }

        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : query.PageSize;

        var totalCount = await tasksQuery.CountAsync();

        var totalPages = totalCount == 0
            ? 0
            : (int)Math.Ceiling(totalCount / (double)pageSize);

        var items = await tasksQuery
            .OrderByDescending(x => x.CreatedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new TaskResponse(
                x.Id,
                x.ProjectId,
                x.Title,
                x.Description,
                x.Status,
                x.Priority,
                x.DueDateUtc,
                x.CreatedAtUtc,
                x.UpdatedAtUtc))
            .ToListAsync();

        return Ok(new PagedResponse<TaskResponse>(
            items,
            page,
            pageSize,
            totalCount,
            totalPages));
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
