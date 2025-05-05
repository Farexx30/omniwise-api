using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Application.AssignmentSubmissionComments.Commands.UpdateAssignmentSubmissionComment;

public class UpdateAssignmentSubmissionCommentCommandValidator : AbstractValidator<UpdateAssignmentSubmissionCommentCommand>
{
    public UpdateAssignmentSubmissionCommentCommandValidator()
    {
        RuleFor(asc => asc.Content)
            .Length(1, 2000)
            .WithMessage("Comment content must contain between 1 and 2000 characters.");
    }
}
