using Azure.Storage.Blobs;
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

    public async Task<string> UploadFileAsync(Stream fileContent, string fileName)
    {
        var blobServiceClient = new BlobServiceClient(_settings.ConnectionString);

        var blobContainerClient = blobServiceClient.GetBlobContainerClient(_settings.ContainerName);
        await blobContainerClient.CreateIfNotExistsAsync();

        var blobClient = blobContainerClient.GetBlobClient(fileName);

        await blobClient.UploadAsync(fileContent);

        var blobUri = blobClient.Uri.ToString();
        return blobUri;
    }

    public async Task DeleteFileAsync(string fileName)
    {
        var blobServiceClient = new BlobServiceClient(_settings.ConnectionString);

        var blobContainerClient = blobServiceClient.GetBlobContainerClient(_settings.ContainerName);

        var blobClient = blobContainerClient.GetBlobClient(fileName);
        await blobClient.DeleteIfExistsAsync();
    }
}
