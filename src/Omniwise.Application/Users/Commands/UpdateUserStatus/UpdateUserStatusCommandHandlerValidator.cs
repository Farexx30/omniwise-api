using FluentValidation;
using Omniwise.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Application.Users.Commands.UpdateUserStatus;

public class UpdateUserStatusCommandHandlerValidator : AbstractValidator<UpdateUserStatusCommand>
{
    public UpdateUserStatusCommandHandlerValidator()
    {
        RuleFor(u => u.Status)
            .IsInEnum()
            .WithMessage("Invalid user status.")
            .Must(s => s != UserStatus.Pending)
            .WithMessage("Not allowed user status.");
    }
}
