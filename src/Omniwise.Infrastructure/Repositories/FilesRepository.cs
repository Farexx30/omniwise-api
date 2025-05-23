using Microsoft.EntityFrameworkCore;
using Omniwise.Application.Common.Interfaces.Repositories;
using Omniwise.Domain.Entities;
using Omniwise.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using File = Omniwise.Domain.Entities.File;

namespace Omniwise.Infrastructure.Repositories;

internal class FilesRepository(OmniwiseDbContext dbContext) : IFilesRepository
{
    //This method deletes orphaned records from File table.
    //It is important when we delete higher in hierarchy entities and cascade delete those who contains files
    //because in such case since we use TPT mapping strategy, for any File type, the File base table
    //is not being affected by such action which would result in orphaned records in this table.
    public async Task DeleteOrphansByBlobNamesAsync(IEnumerable<string> blobNames)
    {
        //Unluckily we cannot use ExecuteDeleteAsync (or any bulk operation provided by EF Core + LINQ)
        //since it is not support with tpt mapping. That's why use raw SQL query here:

        string jsonBlobNames = JsonSerializer.Serialize(blobNames);

        FormattableString query = $@"
            DELETE FROM Files
            WHERE BlobName IN (
                SELECT value
                FROM OPENJSON({jsonBlobNames})
            )";

        await dbContext.Database.ExecuteSqlInterpolatedAsync(query);
    }

    public async Task<List<string>> GetAllBlobNamesByParentIdsAsync<TFile>(IEnumerable<int> parentIds)
        where TFile : File
    {
        var foreignKeyPropertyName = GetForeignKeyPropertyName(typeof(TFile));

        var blobNames = await dbContext.Set<TFile>()
            .Where(f => parentIds.Contains(EF.Property<int>(f, foreignKeyPropertyName)))
            .Select(f => f.BlobName)
            .ToListAsync();

        return blobNames;
    }

    private static string GetForeignKeyPropertyName(Type type)
    {
        return type switch
        {
            var t when t == typeof(LectureFile) => nameof(LectureFile.LectureId),
            var t when t == typeof(AssignmentFile) => nameof(AssignmentFile.AssignmentId),
            var t when t == typeof(AssignmentSubmissionFile) => nameof(AssignmentSubmissionFile.AssignmentSubmissionId),
            _ => throw new Exception($"Type {type.Name} is not valid.")
        };
    }
}
