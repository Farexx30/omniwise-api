using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Application.Users.Dtos;

public class UserDto
{
    public string FullName { get; set; } = default!;
    public string Email { get; set; } = default!;
}
