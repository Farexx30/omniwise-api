using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Application.Extensions;

public static class FormFileExtensions
{
    public static bool IsImage(this IFormFile file)
    {
        var allowedImageExtensions = new[] 
        { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".tiff", ".jiff", ".jfif", ".svg", 
            ".eps", ".bmp", ".raw" };

        var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

        return allowedImageExtensions.Contains(fileExtension);
    }
}
