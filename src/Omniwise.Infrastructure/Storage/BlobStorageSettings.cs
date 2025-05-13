using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Infrastructure.Storage;

public class BlobStorageSettings
{
    public string ConnectionString { get; set; } = default!;
    public string ContainerName { get; set; } = default!;
    public string AccountKey { get; set; } = default!;
}
