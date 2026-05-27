//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using System.IO;

namespace kyxsan.Factory.IO;

internal static class MemoryStreamFactoryExtension
{
    extension(IMemoryStreamFactory memoryStreamFactory)
    {
        public async ValueTask<MemoryStream> GetStreamAsync(Stream copyFrom, bool resetSourcePosition = false)
        {
            MemoryStream targetStream = memoryStreamFactory.GetStream();
            await copyFrom.CopyToAsync(targetStream).ConfigureAwait(false);
            targetStream.Position = 0;
            if (resetSourcePosition && copyFrom.CanSeek)
            {
                copyFrom.Position = 0;
            }

            return targetStream;
        }
    }
}