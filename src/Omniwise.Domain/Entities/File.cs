using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Domain.Entities;

public abstract class File
{
    public int Id { get; set; }
    public string OriginalName { get; set; } = default!;
    public string BlobName { get; set; } = default!;
    public string ContentHash { get; set; } = default!;
}

public class LectureFile : File
{
    public int LectureId { get; set; }
}

public class AssignmentFile : File
{
    public int AssignmentId { get; set; }
}

public class AssignmentSubmissionFile : File
{
    public int AssignmentSubmissionId { get; set; }
}
