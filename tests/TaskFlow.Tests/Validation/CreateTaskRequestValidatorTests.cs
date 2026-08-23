using FluentAssertions;
using TaskFlow.Application.Tasks;
using TaskFlow.Application.Tasks.Validators;
using TaskFlow.Domain.Enums;

namespace TaskFlow.Tests.Validation;

public class CreateTaskRequestValidatorTests
{
    private readonly CreateTaskRequestValidator _validator = new();

    [Fact]
    public async Task Validate_ShouldFail_WhenTitleIsEmpty()
    {
        var request = new CreateTaskRequest(
            "",
            "Description",
            TaskPriority.Medium,
            DateTime.UtcNow.AddDays(1));

        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();

        result.Errors.Should()
            .Contain(x => x.PropertyName == nameof(CreateTaskRequest.Title));
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenPriorityIsInvalid()
    {
        var request = new CreateTaskRequest(
            "Write tests",
            null,
            (TaskPriority)999,
            null);

        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();

        result.Errors.Should()
            .Contain(x => x.PropertyName == nameof(CreateTaskRequest.Priority));
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenDueDateIsInThePast()
    {
        var request = new CreateTaskRequest(
            "Write tests",
            null,
            TaskPriority.High,
            DateTime.UtcNow.AddMinutes(-1));

        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();

        result.Errors.Should()
            .Contain(x => x.PropertyName == nameof(CreateTaskRequest.DueDateUtc));
    }

    [Fact]
    public async Task Validate_ShouldPass_ForValidRequest()
    {
        var request = new CreateTaskRequest(
            "Write tests",
            "Cover create-task validation",
            TaskPriority.High,
            DateTime.UtcNow.AddDays(1));

        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeTrue();
    }
}
