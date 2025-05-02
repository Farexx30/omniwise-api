using Omniwise.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using File = Omniwise.Domain.Entities.File;

namespace Omniwise.Application.Common.Interfaces
{
    public interface IFilesRepository
    {
        Task CreateManyAsync(IEnumerable<File> files);
        Task DeleteManyAsync(IEnumerable<File> files);
    }
}
