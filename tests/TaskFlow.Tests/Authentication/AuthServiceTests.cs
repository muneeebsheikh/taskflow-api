using FluentAssertions;
using Moq;
using TaskFlow.Application.Authentication;
using TaskFlow.Application.Common.Exceptions;
using TaskFlow.Application.Interfaces;
using TaskFlow.Infrastructure.Authentication;
using TaskFlow.Tests.Helpers;

namespace TaskFlow.Tests.Authentication;

public class AuthServiceTests
{
    private readonly Mock<IJwtTokenGenerator> _tokenGenerator = new();

    [Fact]
    public async Task RegisterAsync_ShouldCreateUser()
    {
        await using var dbContext = TestDbContextFactory.Create();

        _tokenGenerator
            .Setup(x => x.GenerateToken(It.IsAny<TaskFlow.Domain.Entities.User>()))
            .Returns("test-token");

        var service = new AuthService(
            dbContext,
            new PasswordHasher(),
            _tokenGenerator.Object);

        var request = new RegisterRequest(
            "Muhammad",
            "Muneeb",
            "Muneeb@Test.com",
            "StrongPassword123!");

        var response = await service.RegisterAsync(request);

        response.Email.Should().Be("muneeb@test.com");
        response.Token.Should().Be("test-token");

        dbContext.Users.Should().HaveCount(1);

        var user = dbContext.Users.Single();

        user.FirstName.Should().Be("Muhammad");
        user.PasswordHash.Should().NotBe("StrongPassword123!");
    }

    [Fact]
    public async Task RegisterAsync_ShouldThrowConflict_WhenEmailAlreadyExists()
    {
        await using var dbContext = TestDbContextFactory.Create();

        _tokenGenerator
            .Setup(x => x.GenerateToken(It.IsAny<TaskFlow.Domain.Entities.User>()))
            .Returns("test-token");

        var service = new AuthService(
            dbContext,
            new PasswordHasher(),
            _tokenGenerator.Object);

        var request = new RegisterRequest(
            "Muhammad",
            "Muneeb",
            "muneeb@test.com",
            "StrongPassword123!");

        await service.RegisterAsync(request);

        var action = async () =>
            await service.RegisterAsync(request);

        await action.Should()
            .ThrowAsync<ConflictException>();
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnToken_ForValidCredentials()
    {
        await using var dbContext = TestDbContextFactory.Create();

        _tokenGenerator
            .Setup(x => x.GenerateToken(It.IsAny<TaskFlow.Domain.Entities.User>()))
            .Returns("login-token");

        var service = new AuthService(
            dbContext,
            new PasswordHasher(),
            _tokenGenerator.Object);

        await service.RegisterAsync(
            new RegisterRequest(
                "Muhammad",
                "Muneeb",
                "muneeb@test.com",
                "StrongPassword123!"));

        var response = await service.LoginAsync(
            new LoginRequest(
                "muneeb@test.com",
                "StrongPassword123!"));

        response.Token.Should().Be("login-token");
    }

    [Fact]
    public async Task LoginAsync_ShouldThrowUnauthorized_ForWrongPassword()
    {
        await using var dbContext = TestDbContextFactory.Create();

        _tokenGenerator
            .Setup(x => x.GenerateToken(It.IsAny<TaskFlow.Domain.Entities.User>()))
            .Returns("test-token");

        var service = new AuthService(
            dbContext,
            new PasswordHasher(),
            _tokenGenerator.Object);

        await service.RegisterAsync(
            new RegisterRequest(
                "Muhammad",
                "Muneeb",
                "muneeb@test.com",
                "StrongPassword123!"));

        var action = async () =>
            await service.LoginAsync(
                new LoginRequest(
                    "muneeb@test.com",
                    "WrongPassword123!"));

        await action.Should()
            .ThrowAsync<UnauthorizedAccessException>();
    }
}
