using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Domain.Entities;

public class LectureFile
{
    public int Id { get; set; }
    public string Url { get; set; } = default!;
}
