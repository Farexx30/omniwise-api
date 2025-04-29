using Omniwise.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Infrastructure.Persistence;

internal class UnitOfWork(OmniwiseDbContext dbContext) : IUnitOfWork
{
    public async Task ExecuteTransactionalAsync(Func<Task> action)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            await action();

            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();

            throw new Exception(ex.Message);
        }
    }
}
