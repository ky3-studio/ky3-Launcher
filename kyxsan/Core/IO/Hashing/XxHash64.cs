//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using System.Buffers;
using System.IO;

namespace kyxsan.Core.IO.Hashing;

internal static class XxHash64
{
    public static async ValueTask<string> HashAsync(Stream stream, CancellationToken token = default)
    {
        System.IO.Hashing.XxHash64 xxHash64 = new();
        await xxHash64.AppendAsync(stream, token).ConfigureAwait(false);

        byte[] bytes = ArrayPool<byte>.Shared.Rent(xxHash64.HashLengthInBytes);
        try
        {
            xxHash64.TryGetHashAndReset(bytes, out int bytesWritten);
            Verify.Operation(bytesWritten == xxHash64.HashLengthInBytes, "Hash length is wrong");
            return Convert.ToHexString(bytes.AsSpan(0, bytesWritten));
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(bytes);
        }
    }

    public static async ValueTask<string> HashFileAsync(string path, CancellationToken token = default)
    {
        using (FileStream stream = File.OpenRead(path))
        {
            return await HashAsync(stream, token).ConfigureAwait(false);
        }
    }
}