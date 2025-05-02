using Microsoft.AspNetCore.Http;
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
    private readonly Stack<Func<Task>> _rollbackOperations = [];
    private readonly List<string> _createdSnapshotsFileNames = [];

    public async Task<TFile> UploadFileAsync<TFile>(IFormFile file, int parentId)
        where TFile : File, new()
    {
        var folderName = GetFolderName(typeof(TFile));
        var fileName = $"{folderName}/{parentId}-{file.FileName}";

        logger.LogInformation("Uploading file {fileName}.", file.FileName);

        //TRY to create a snapshot of the blob to be able to restore it later if needed.
        //The snapshot will be created only when blob already exists - if it is not the reverse operation is delete then.
        var snapshot = await blobStorageService.CreateBlobSnapshotAsync(fileName);
        if (snapshot is not null)
        {
            _rollbackOperations.Push(() => blobStorageService.RestoreBlobFromSnapshotAsync(fileName, snapshot));
            _createdSnapshotsFileNames.Add(fileName);
        }
        else
        {
            _rollbackOperations.Push(() => blobStorageService.DeleteBlobAsync(fileName));
        }

        using var stream = file.OpenReadStream();
        var contentHash = OmniwiseCryptography.ComputeSha256Hash(stream);
        await blobStorageService.UploadBlobAsync(stream, fileName);

        return new TFile
        {
            Name = fileName,
            ContentHash = contentHash,
        };
    }

    public async Task DeleteFileAsync(string fileName)
    {
        logger.LogInformation("Deleting file {fileName}.", fileName);

        _rollbackOperations.Push(() => blobStorageService.RestoreFromTrashAsync(fileName));

        await blobStorageService.MoveToTrashAsync(fileName);
    }

    public string GetFileSasUrl(string fileName)
    {
        var fileSasUrl = blobStorageService.GetBlobSasUrl(fileName);
        return fileSasUrl;
    }

    public async Task CleanUpAsync()
    {
        foreach (var fileName in _createdSnapshotsFileNames)
        {
            await blobStorageService.DeleteBlobSnapshotsAsync(fileName);
        }
        _createdSnapshotsFileNames.Clear();
      
        await blobStorageService.ClearTrashAsync();
    }

    public async Task RollbackChangesAsync()
    {
        while (_rollbackOperations.Count != 0)
        {
            var rollbackOperation = _rollbackOperations.Pop();
            await rollbackOperation();
        }
    }

    public void ValidateFiles(IEnumerable<IFormFile> files)
    {
        //Check if there is at least one file in the collection:
        if (!files.Any())
        {
            throw new BadRequestException("At least one file is required.");
        }

        //Check if all files have unique names:
        var hasDuplicatedNames = files
            .GroupBy(f => f.FileName)
            .Any(g => g.Count() > 1);

        if (hasDuplicatedNames)
        {
            throw new BadRequestException("Files with the exact same name and extension are not allowed.");
        }

        //Check if any file name is not too long:
        var isAnyFileNameTooLong = files
            .Any(f => f.FileName.Length > 256);

        if (isAnyFileNameTooLong)
        {
            throw new BadHttpRequestException("File name is too long. Maximum length is 256 characters.");
        }
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