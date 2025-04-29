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
    public async Task<TFile> UploadFileAsync<TFile>(IFormFile file, int parentId)
        where TFile : File, new()
    {
        var folderName = GetFolderName(typeof(TFile));

        var fileName = $"{folderName}/{parentId}-{file.FileName}";

        logger.LogInformation("Uploading file {fileName}.",
            file.FileName);

        using var stream = file.OpenReadStream();
        var contentHash = OmniwiseCryptography.ComputeSha256Hash(stream);
        var fileUrl = await blobStorageService.UploadFileAsync(stream, fileName);

        return new TFile
        {
            Name = fileName,
            Url = fileUrl,
            ContentHash = contentHash,
        };
    }

    public async Task DeleteFileAsync(string fileName)
    {
        logger.LogInformation("Deleting file {fileName}.", fileName);

        await blobStorageService.DeleteFileAsync(fileName);
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
