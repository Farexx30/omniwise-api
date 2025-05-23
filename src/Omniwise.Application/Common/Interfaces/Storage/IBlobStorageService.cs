using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Application.Common.Interfaces.Storage;

public interface IBlobStorageService
{
    Task UploadBlobAsync(Stream blobContent, string blobName);
    Task DeleteBlobAsync(string blobName);
    Task<string> GetBlobSasUrl(string blobName);
    Task CreateBlobContainerIfNotExistsAsync();
}
