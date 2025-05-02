using Omniwise.Application.Common.Interfaces;
using Omniwise.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using File = Omniwise.Domain.Entities.File;

namespace Omniwise.Infrastructure.Repositories;

internal class FilesRepository(OmniwiseDbContext dbContext) : IFilesRepository
{
    public async Task CreateManyAsync(IEnumerable<File> files)
    {
        dbContext.Files.AddRange(files);
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteManyAsync(IEnumerable<File> files)
    {
        dbContext.Files.RemoveRange(files);
        await dbContext.SaveChangesAsync();
    }
}
