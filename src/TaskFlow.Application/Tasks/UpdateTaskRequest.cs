using TaskFlow.Domain.Enums;

namespace TaskFlow.Application.Tasks;

public sealed record UpdateTaskRequest(
    string Title,
    string? Description,
    TaskItemStatus Status,
    TaskPriority Priority,
    DateTime? DueDateUtc);
