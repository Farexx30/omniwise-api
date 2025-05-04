using Microsoft.AspNetCore.Http;
using Omniwise.Application.Common.Types;
using Omniwise.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Application.Common.Static;


public static class OmniwiseFileValidation
{
    public static OmniwiseValidationResult Validate(List<IFormFile> files)
    {
        var errors = new List<string>();
        bool isSuccess = true;

        //Check if user sent at least one file:
        if (files.Count == 0)
        {
            errors.Add("At least one file is required.");
            isSuccess = false;
        }

        //Check if all files have unique names:
        var hasDuplicatedNames = files
            .GroupBy(f => f.FileName)
            .Any(g => g.Count() > 1);

        if (hasDuplicatedNames)
        {
            errors.Add("Files with the exact same name and extension are not allowed.");
            isSuccess = false;
        }

        //Check if any file name is not too long:
        var isAnyFileNameTooLong = files
            .Any(f => f.FileName.Length > 256);

        if (isAnyFileNameTooLong)
        {
            errors.Add("Some file names are too long. Maximum length is 256 characters.");
            isSuccess = false;
        }

        return new OmniwiseValidationResult
        {
            Succeeded = isSuccess,
            Errors = errors
        };
    }
}
