using FluentValidation;
using Omniwise.Application.Common.Static;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Application.Assignments.Commands.CreateAssignment;

public class CreateAssignmentCommandValidator : AbstractValidator<CreateAssignmentCommand>
{
    public CreateAssignmentCommandValidator()
    {
        RuleFor(a => a.Name)
            .Length(3, 256);

        RuleFor(a => a.Deadline)
            .GreaterThan(DateTime.UtcNow)
            .WithMessage("Deadline must be in the future.");

        RuleFor(a => a.MaxGrade)
            .GreaterThan(0f);

        RuleFor(a => a.Files)
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
