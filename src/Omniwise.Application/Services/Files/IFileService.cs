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
    Task<TFile> UploadFileAsync<TFile>(IFormFile file, int parentId)
        where TFile : File, new();
    Task DeleteFileAsync(string fileName);
    string GetFileSasUrl(string fileName);
    void ValidateFiles(IEnumerable<IFormFile> files);
}
