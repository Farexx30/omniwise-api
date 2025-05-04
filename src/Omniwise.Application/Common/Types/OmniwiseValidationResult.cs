using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Application.Common.Types;

public class OmniwiseValidationResult
{
    public bool Succeeded { get; init; } = false;
    public IEnumerable<string> Errors { get; init; } = [];
}
