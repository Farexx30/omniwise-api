using FluentValidation;

namespace Omniwise.Application.Lectures.Commands.UpdateLecture;

public class UpdateLectureCommandValidator : AbstractValidator<UpdateLectureCommand>
{
    public UpdateLectureCommandValidator()
    {
        RuleFor(l => l.Name)
            .Length(3, 256);
    }
}