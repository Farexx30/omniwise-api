using FluentValidation;

namespace Omniwise.Application.Lectures.Commands.CreateLecture;

public class CreateLectureCommandValidator : AbstractValidator<CreateLectureCommand>
{
    public CreateLectureCommandValidator()
    {
        RuleFor(l => l.Name)
            .Length(3, 256);
    }
}