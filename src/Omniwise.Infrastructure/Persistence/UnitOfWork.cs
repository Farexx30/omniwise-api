using Microsoft.Extensions.Logging;
using Omniwise.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Infrastructure.Persistence;

internal class UnitOfWork(OmniwiseDbContext dbContext, ILogger<UnitOfWork> logger) : IUnitOfWork
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

            logger.LogError(ex, "An error occurred while executing a transactional operation.");

            throw new Exception("An unexpected error occured while saving your changes.");
        }
    }
}
