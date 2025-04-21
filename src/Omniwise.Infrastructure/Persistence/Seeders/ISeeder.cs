using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Infrastructure.Persistence.Seeders;

public interface ISeeder
{
    Task SeedAsync();
}

public interface ISeeder<TEntity> : ISeeder
    where TEntity : class
{
}
