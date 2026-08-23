using FluentAssertions;
using TaskFlow.Application.Authentication;
using TaskFlow.Application.Authentication.Validators;

namespace TaskFlow.Tests.Validation;

public class RegisterRequestValidatorTests
{
    private readonly RegisterRequestValidator _validator = new();

    [Fact]
    public async Task Validate_ShouldFail_WhenEmailIsInvalid()
    {
        var request = new RegisterRequest(
            "Muhammad",
            "Muneeb",
            "not-an-email",
            "StrongPassword123!");

        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();

        result.Errors.Should()
            .Contain(x => x.PropertyName == nameof(RegisterRequest.Email));
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenPasswordIsWeak()
    {
        var request = new RegisterRequest(
            "Muhammad",
            "Muneeb",
            "muneeb@test.com",
            "123");

        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task Validate_ShouldPass_ForValidRequest()
    {
        var request = new RegisterRequest(
            "Muhammad",
            "Muneeb",
            "muneeb@test.com",
            "StrongPassword123!");

        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeTrue();
    }
}
