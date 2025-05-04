using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using File = Omniwise.Domain.Entities.File;

namespace Omniwise.Application.Services.Files;

public interface IFileService
{
    Task<List<TFile>> UploadAllAsync<TFile>(List<IFormFile> files, int parentId)
        where TFile : File, new();
    Task CompareAndUpdateAsync<TFile>(List<IFormFile> newFiles, List<TFile> currentFiles, int parentId)
        where TFile : File, new();
    Task DeleteAllAsync(List<string> fileNames);
    Task<string> GetFileSasUrl(string fileName);
    void ValidateFiles(IEnumerable<IFormFile> files);
}
