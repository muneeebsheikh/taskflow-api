using FluentAssertions;
using TaskFlow.Infrastructure.Authentication;

namespace TaskFlow.Tests.Authentication;

public class PasswordHasherTests
{
    private readonly PasswordHasher _hasher = new();

    [Fact]
    public void Hash_ShouldNotReturnPlainTextPassword()
    {
        const string password = "StrongPassword123!";

        var hash = _hasher.Hash(password);

        hash.Should().NotBe(password);
    }

    [Fact]
    public void Verify_ShouldReturnTrue_ForCorrectPassword()
    {
        const string password = "StrongPassword123!";

        var hash = _hasher.Hash(password);

        var result = _hasher.Verify(password, hash);

        result.Should().BeTrue();
    }

    [Fact]
    public void Verify_ShouldReturnFalse_ForIncorrectPassword()
    {
        var hash = _hasher.Hash("StrongPassword123!");

        var result = _hasher.Verify(
            "WrongPassword123!",
            hash);

        result.Should().BeFalse();
    }
}
