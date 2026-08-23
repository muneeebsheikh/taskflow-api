namespace TaskFlow.Application.Authentication;

public sealed record AuthResponse(
    Guid UserId,
    string FirstName,
    string LastName,
    string Email,
    string Token);
