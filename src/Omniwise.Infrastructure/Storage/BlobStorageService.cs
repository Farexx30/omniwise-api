using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.Extensions.Options;
using Omniwise.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Infrastructure.Storage;

internal class BlobStorageService(IOptions<BlobStorageSettings> settingsOptions) : IBlobStorageService
{
    private readonly BlobStorageSettings _settings = settingsOptions.Value;

    public async Task<string> UploadFileAsync(Stream fileContent, string blobName)
    {
        var blobClient = GetBlobClient(blobName);

        await blobClient.UploadAsync(fileContent);

        var blobUri = blobClient.Uri.ToString();
        return blobUri;
    }

    public async Task DeleteFileAsync(string blobName)
    {
        var blobClient = GetBlobClient(blobName);
        await blobClient.DeleteIfExistsAsync();
    }

    public string GetBlobSasUrl(string blobName)
    {   
        var blobClient = GetBlobClient(blobName);

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
