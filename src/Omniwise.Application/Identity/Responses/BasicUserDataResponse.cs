using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Application.Identity.Responses;

public class BasicUserDataResponse
{
    public required string UserId { get; set; }
    public required string Role { get; init; }
}
