using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Application.Common.Static;

public static class OmniwiseCryptography
{
    public static string ComputeSha256Hash(Stream stream)
    {
        using var sha256 = SHA256.Create();
        stream.Position = 0;
        var hash = sha256.ComputeHash(stream);
        stream.Position = 0;
        return Convert.ToHexString(hash);
    }
}
