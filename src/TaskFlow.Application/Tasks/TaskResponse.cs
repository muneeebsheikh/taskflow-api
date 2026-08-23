using TaskFlow.Domain.Enums;

namespace TaskFlow.Application.Tasks;

public sealed record TaskResponse(
    Guid Id,
    Guid ProjectId,
    string Title,
    string? Description,
    TaskItemStatus Status,
    TaskPriority Priority,
    DateTime? DueDateUtc,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);
