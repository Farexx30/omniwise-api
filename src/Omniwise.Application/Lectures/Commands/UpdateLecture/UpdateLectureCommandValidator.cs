using FluentValidation;
using Omniwise.Application.Common.Static;

namespace Omniwise.Application.Lectures.Commands.UpdateLecture;

public class UpdateLectureCommandValidator : AbstractValidator<UpdateLectureCommand>
{
    public UpdateLectureCommandValidator()
    {
        RuleFor(l => l.Name)
            .Length(3, 256);

        RuleFor(l => l.Content)
            .Must(content => content is null || content.Length <= 4500)
            .WithMessage("Content must be 4500 characters or fewer.");

        RuleFor(l => l.Files)
            .Custom((value, context) =>
            {
                var validationResult = OmniwiseFileValidation.Validate(value);
                if (!validationResult.Succeeded)
                {
                    foreach (var error in validationResult.Errors)
                    {
                        context.AddFailure("Files", error);
                    }
                }
            });
    }
}