using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Infrastructure.Seeders;

public interface ISeeder<TEntity>
    where TEntity : class
{
    Task SeedAsync();
}
