using FluentValidation;

namespace TaskFlow.Application.Tasks.Validators;

public class CreateTaskRequestValidator
    : AbstractValidator<CreateTaskRequest>
{
    public CreateTaskRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(250);

        RuleFor(x => x.Description)
            .MaximumLength(4000);

        RuleFor(x => x.Priority)
            .IsInEnum();

        RuleFor(x => x.DueDateUtc)
            .GreaterThan(DateTime.UtcNow)
            .When(x => x.DueDateUtc.HasValue)
            .WithMessage("Due date must be in the future.");
    }
}
