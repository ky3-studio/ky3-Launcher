//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace kyxsan.Core.IO.Hashing;

internal static class Hash
{
    public static string ToHexString(HashAlgorithmName hashAlgorithm, string input)
    {
        return Convert.ToHexString(CryptographicOperations.HashData(hashAlgorithm, Encoding.UTF8.GetBytes(input)));
    }

    public static string ToHexStringLower(HashAlgorithmName hashAlgorithm, string input)
    {
        return Convert.ToHexStringLower(CryptographicOperations.HashData(hashAlgorithm, Encoding.UTF8.GetBytes(input)));
    }

    public static string ToHexString(HashAlgorithmName hashAlgorithm, ReadOnlySpan<byte> input)
    {
        return Convert.ToHexString(CryptographicOperations.HashData(hashAlgorithm, input));
    }

    public static async ValueTask<string> ToHexStringAsync(HashAlgorithmName hashAlgorithm, Stream input, CancellationToken token = default)
    {
        return Convert.ToHexString(await CryptographicOperations.HashDataAsync(hashAlgorithm, input, token).ConfigureAwait(false));
    }

    public static async ValueTask<string> FileToHexStringAsync(HashAlgorithmName hashAlgorithm, string filePath, CancellationToken token = default)
    {
        using (FileStream stream = File.OpenRead(filePath))
        {
            return await ToHexStringAsync(hashAlgorithm, stream, token).ConfigureAwait(false);
        }
    }
}