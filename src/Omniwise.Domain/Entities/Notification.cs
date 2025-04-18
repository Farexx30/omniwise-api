using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Domain.Entities;

public class Notification
{
    public int Id { get; set; }
    public string Content { get; set; } = default!;
    public DateTime SentDate { get; set; }

    public string UserId { get; set; } = default!;
    public User User { get; set; } = default!;
}
