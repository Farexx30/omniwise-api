using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using File = Omniwise.Domain.Entities.File;

namespace Omniwise.Application.Common.Interfaces.Repositories;

public interface IFilesRepository
{
    Task DeleteOrphansByBlobNamesAsync(IEnumerable<string> blobNames);
    Task<List<string>> GetAllBlobNamesByParentIdsAsync<TFile>(IEnumerable<int> parentIds)
        where TFile : File;
}
