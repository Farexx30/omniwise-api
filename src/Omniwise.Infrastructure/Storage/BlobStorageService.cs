using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Omniwise.Application.Common.Interfaces;
using Omniwise.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Infrastructure.Storage;

internal class BlobStorageService(IOptions<BlobStorageSettings> settingsOptions,
    ILogger<BlobStorageService> logger) : IBlobStorageService
{
    private readonly BlobStorageSettings _settings = settingsOptions.Value;

    public async Task UploadBlobAsync(Stream fileContent, string blobName)
    {
        var blobClient = GetBlobClient(blobName);
        await blobClient.UploadAsync(fileContent, overwrite: true);
    }

    public async Task DeleteBlobAsync(string blobName)
    {
        var blobClient = GetBlobClient(blobName);
        await blobClient.DeleteIfExistsAsync();
    }

    public async Task<string> GetBlobSasUrl(string blobName)
    {
        var blobClient = GetBlobClient(blobName);

        if (!await blobClient.ExistsAsync())
        {
            //This error should never happen!
            //But if, it means that somehow database is not in sync with blob storage.
            logger.LogCritical("Blob with {blobName} not found.", blobName); 
            throw new NotFoundException($"Blob with {blobName} not found.");
        }

        var blobSasUri = blobClient.GenerateSasUri(BlobSasPermissions.Read, DateTimeOffset.UtcNow.AddHours(1));
        return blobSasUri.ToString();
    }

    public async Task CreateBlobContainerIfNotExistsAsync()
    {
        var blobServiceClient = new BlobServiceClient(_settings.ConnectionString);

        var blobContainerClient = blobServiceClient.GetBlobContainerClient(_settings.ContainerName);
        await blobContainerClient.CreateIfNotExistsAsync();
    }

    private BlobClient GetBlobClient(string blobName)
    {
        var blobServiceClient = new BlobServiceClient(_settings.ConnectionString);

        var blobContainerClient = blobServiceClient.GetBlobContainerClient(_settings.ContainerName);

        var blobClient = blobContainerClient.GetBlobClient(blobName);
        return blobClient;
    }
}