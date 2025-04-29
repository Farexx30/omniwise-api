using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Application.Common.Interfaces;

public interface IBlobStorageService
{
    Task<string> UploadFileAsync(Stream fileContent, string fileName);
    Task DeleteFileAsync(string fileName);
}
