//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

namespace kyxsan.Core.IO;

internal sealed class StreamCopyStatus
{
    public StreamCopyStatus(long bytesReadSinceLastReport, long bytesReadSinceCopyStart, long totalBytes)
    {
        BytesReadSinceLastReport = bytesReadSinceLastReport;
        BytesReadSinceCopyStart = bytesReadSinceCopyStart;
        TotalBytes = totalBytes;
    }

    public long BytesReadSinceLastReport { get; }

    public long BytesReadSinceCopyStart { get; }

    public long TotalBytes { get; }
}