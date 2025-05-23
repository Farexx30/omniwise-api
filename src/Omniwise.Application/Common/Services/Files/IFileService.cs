﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using File = Omniwise.Domain.Entities.File;

namespace Omniwise.Application.Common.Services.Files;

public interface IFileService
{
    Task<List<TFile>> UploadAllAsync<TFile>(IEnumerable<IFormFile> files, int parentId)
        where TFile : File, new();
    Task CompareAndUpdateAsync<TFile>(IEnumerable<IFormFile> newFiles, List<TFile> currentFiles, int parentId)
        where TFile : File, new();
    Task DeleteAllAsync(List<string> fileNames);
    Task<string> GetFileSasUrl(string fileName);
}
