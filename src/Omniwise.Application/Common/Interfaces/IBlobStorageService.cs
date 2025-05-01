using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Application.Common.Interfaces;

public interface IBlobStorageService
{
    Task<string> UploadFileAsync(Stream blobContent, string blobName);
    Task DeleteFileAsync(string blobName);
    string GetBlobSasUrl(string blobName);
    Task CreateBlobContainerIfNotExistsAsync();
}
