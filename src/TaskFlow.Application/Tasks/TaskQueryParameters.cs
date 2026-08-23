using TaskFlow.Domain.Enums;

namespace TaskFlow.Application.Tasks;

public class TaskQueryParameters
{
    private const int MaxPageSize = 100;

    private int _pageSize = 20;

    public int Page { get; set; } = 1;

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize
            ? MaxPageSize
            : value;
    }

    public TaskItemStatus? Status { get; set; }

    public TaskPriority? Priority { get; set; }

    public string? Search { get; set; }
}
