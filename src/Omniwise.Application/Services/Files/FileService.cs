using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using Omniwise.Application.AssignmentSubmissions.Commands.CreateAssignmentSubmission;
using Omniwise.Application.Common.Interfaces;
using Omniwise.Application.Common.Static;
using Omniwise.Application.Common.Types;
using Omniwise.Domain.Constants;
using Omniwise.Domain.Entities;
using Omniwise.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using File = Omniwise.Domain.Entities.File;

namespace Omniwise.Application.Services.Files;

internal class FileService(ILogger<FileService> logger,
        IBlobStorageService blobStorageService) : IFileService
{
    public async Task<List<TFile>> UploadAllAsync<TFile>(List<IFormFile> files, int parentId)
        where TFile : File, new()
    {
        var folderName = GetFolderName(typeof(TFile));

        logger.LogInformation("Uploading files to {folderName} folder.", folderName);

        List<TFile> uploadedFiles = [];
        foreach (var file in files)
        {
            var originalFileName = file.FileName;
            var blobName = $"{folderName}/{parentId}-{originalFileName}";

            using var stream = file.OpenReadStream();
            var contentHash = OmniwiseCryptography.ComputeSha256Hash(stream);
            await blobStorageService.UploadBlobAsync(stream, blobName);

            uploadedFiles.Add(new TFile
            {
                OriginalName = originalFileName,
                BlobName = blobName,
                ContentHash = contentHash,
            });
        }

        return uploadedFiles;
    }

    public async Task CompareAndUpdateAsync<TFile>(List<IFormFile> newFiles, List<TFile> currentFiles, int parentId)
        where TFile : File, new()
    {
        var folderName = GetFolderName(typeof(TFile));

        var oldFiles = currentFiles.ToList();
        foreach (var newFile in newFiles)
        {
            var newFileOriginalName = newFile.FileName;
            var newFileBlobName = $"{folderName}/{parentId}-{newFileOriginalName}";

            using var stream = newFile.OpenReadStream();
            var newFileContentHash = OmniwiseCryptography.ComputeSha256Hash(stream);

            var pairedByContentHashOldFile = oldFiles.FirstOrDefault(cf => cf.ContentHash == newFileContentHash);
            var pairedByOriginalNameOldFile = oldFiles.FirstOrDefault(of => of.OriginalName == newFileOriginalName);

            //If the new file is exactly the same as the old one, we can skip uploading it:
            if (pairedByContentHashOldFile is not null
                && pairedByContentHashOldFile == pairedByOriginalNameOldFile)
            {
                oldFiles.Remove(pairedByContentHashOldFile);
                continue;
            }  

            await blobStorageService.UploadBlobAsync(stream, newFileBlobName);

            //If there is already a record with such name, just perform an update without unnecessary insert + delete.
            if (pairedByOriginalNameOldFile is not null)
            {
                pairedByOriginalNameOldFile.ContentHash = newFileContentHash;
                oldFiles.Remove(pairedByOriginalNameOldFile);
                continue;
            }

            currentFiles.Add(new TFile
            {
                OriginalName = newFileOriginalName,
                BlobName = newFileBlobName,
                ContentHash = newFileContentHash,
            });
        }

        //Delete any "leftovers" if needed:
        foreach (var oldFile in oldFiles)
        {
            await blobStorageService.DeleteBlobAsync(oldFile.BlobName);
            currentFiles.Remove(oldFile);
        }
    }

    public async Task DeleteAllAsync(List<string> fileNames)
    {
        foreach (var fileName in fileNames)
        {
            await blobStorageService.DeleteBlobAsync(fileName);
        }
    }

    public async Task<string> GetFileSasUrl(string fileName)
    {
        var fileSasUrl = await blobStorageService.GetBlobSasUrl(fileName);
        return fileSasUrl;
    }

    private static string GetFolderName(Type fileType)
    {
        return fileType switch
        {
            var type when type == typeof(AssignmentSubmissionFile) => FileFolders.AssignmentSubmissions,
            _ => throw new Exception("Unknown file type.")
        };
    }
}