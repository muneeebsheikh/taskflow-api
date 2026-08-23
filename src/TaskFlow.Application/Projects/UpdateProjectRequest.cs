namespace TaskFlow.Application.Projects;

public sealed record UpdateProjectRequest(
    string Name,
    string? Description);
