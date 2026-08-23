using TaskFlow.Domain.Enums;

namespace TaskFlow.Application.Tasks;

public sealed record CreateTaskRequest(
    string Title,
    string? Description,
    TaskPriority Priority,
    DateTime? DueDateUtc);
