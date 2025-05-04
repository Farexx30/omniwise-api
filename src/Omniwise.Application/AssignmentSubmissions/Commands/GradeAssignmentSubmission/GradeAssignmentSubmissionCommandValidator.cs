using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Application.AssignmentSubmissions.Commands.GradeAssignmentSubmission;

public class GradeAssignmentSubmissionCommandValidator : AbstractValidator<GradeAssignmentSubmissionCommand>
{
    public GradeAssignmentSubmissionCommandValidator()
    {
        RuleFor(a => a.Grade)
            .Must(grade => grade is null || grade >= 0)
            .WithMessage("Grade must be greater than or equal to 0.");
    }
}
