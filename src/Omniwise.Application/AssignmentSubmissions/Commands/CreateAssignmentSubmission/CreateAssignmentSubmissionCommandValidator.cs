using FluentValidation;
using Omniwise.Application.Common.Static;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Application.AssignmentSubmissions.Commands.CreateAssignmentSubmission;

public class CreateAssignmentSubmissionCommandValidator : AbstractValidator<CreateAssignmentSubmissionCommand>
{
    public CreateAssignmentSubmissionCommandValidator()
    {
        RuleFor(asub => asub.Files)
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
