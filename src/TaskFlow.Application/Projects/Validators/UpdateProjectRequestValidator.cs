using FluentValidation;

namespace TaskFlow.Application.Projects.Validators;

public class UpdateProjectRequestValidator
    : AbstractValidator<UpdateProjectRequest>
{
    public UpdateProjectRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .MaximumLength(2000);
    }
}
