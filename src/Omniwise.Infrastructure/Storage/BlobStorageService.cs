using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
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
    private const string TempTrashFolderName = "temp-trash";
    private readonly BlobStorageSettings _settings = settingsOptions.Value;

    public async Task UploadBlobAsync(Stream fileContent, string blobName)
    {
        var blobClient = GetBlobClient(blobName);
        await blobClient.UploadAsync(fileContent);
    }

    public async Task DeleteBlobAsync(string blobName)
    {
        var blobClient = GetBlobClient(blobName);
        await blobClient.DeleteAsync();
    }

    public async Task<string?> CreateBlobSnapshotAsync(string blobName)
    {
        var blobClient = GetBlobClient(blobName);

        if (!await blobClient.ExistsAsync())
        {
            return null;
        }

        var snapshotResponse = await blobClient.CreateSnapshotAsync();
        return snapshotResponse.Value.Snapshot;
    }

    public async Task RestoreBlobFromSnapshotAsync(string blobName, string snapshot)
    {
        var blobClient = GetBlobClient(blobName);

        var snapshotClient = blobClient.WithSnapshot(snapshot);

        using var snapshotStream = await snapshotClient.OpenReadAsync();
        await blobClient.UploadAsync(snapshotStream);

        await snapshotClient.DeleteAsync();
    }

    public async Task DeleteBlobSnapshotsAsync(string blobName)
    {
        var blobClient = GetBlobClient(blobName);
        await blobClient.DeleteAsync(DeleteSnapshotsOption.OnlySnapshots);
    }

    public async Task MoveToTrashAsync(string blobName)
    {
        var sourceBlobClient = GetBlobClient(blobName);
        var destinationBlobName = GetBlobClient($"{TempTrashFolderName}/{blobName}");

        var copyOperation = await destinationBlobName.StartCopyFromUriAsync(sourceBlobClient.Uri);
        await copyOperation.WaitForCompletionAsync();

        await sourceBlobClient.DeleteAsync();
    }

    public async Task RestoreFromTrashAsync(string blobName)
    {
        var sourceBlobClient = GetBlobClient($"{TempTrashFolderName}/{blobName}");
        var destinationBlobName = GetBlobClient(blobName);

        var copyOperation = await destinationBlobName.StartCopyFromUriAsync(sourceBlobClient.Uri);
        await copyOperation.WaitForCompletionAsync();

        await sourceBlobClient.DeleteAsync();
    }

    public async Task ClearTrashAsync()
    {
        var blobServiceClient = new BlobServiceClient(_settings.ConnectionString);

        var blobContainerClient = blobServiceClient.GetBlobContainerClient(_settings.ContainerName);

        var trashBlobs = blobContainerClient.GetBlobsAsync(prefix: TempTrashFolderName);
        await foreach (var blob in trashBlobs)
        {
            var blobClient = blobContainerClient.GetBlobClient(blob.Name);
            await blobClient.DeleteAsync();
        }
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