using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Infrastructure.Persistence.MigrationAppliers
{
    public interface IMigrationApplier
    {
        Task ApplyAsync();
    }
}
