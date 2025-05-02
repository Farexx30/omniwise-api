using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Application.Common.Interfaces;

public interface IBlobStorageService
{
    Task UploadBlobAsync(Stream blobContent, string blobName);
    Task DeleteBlobAsync(string blobName);
    Task<string?> CreateBlobSnapshotAsync(string blobName);
    Task RestoreBlobFromSnapshotAsync(string blobName, string snapshot);
    Task DeleteBlobSnapshotsAsync(string blobName);
    Task MoveToTrashAsync(string blobName);
    Task RestoreFromTrashAsync(string blobName);
    Task ClearTrashAsync();
    string GetBlobSasUrl(string blobName);
    Task CreateBlobContainerIfNotExistsAsync();
}
