using FluentValidation;
using Omniwise.Application.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Application.Courses.Commands.UpdateCourse;

public class UpdateCourseCommandValidator : AbstractValidator<UpdateCourseCommand>
{
    public UpdateCourseCommandValidator()
    {
        RuleFor(c => c.Name)
            .Length(3, 256);

        RuleFor(c => c.Img)
            .Custom((value, context) =>
            {
                if (value is not null)
                {
                    if (!value.IsImage())
                    {
                        context.AddFailure("Img", "Not allowed file extension.");
                    }

                    if (value.FileName.Length > 1024)
                    {
                        context.AddFailure("Img", "Img file name is too long.");
                    }
                }         
            });
    }
}
