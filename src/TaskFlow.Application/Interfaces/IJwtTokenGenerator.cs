using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}
