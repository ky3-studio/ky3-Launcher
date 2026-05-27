//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using CommunityToolkit.Common;

namespace kyxsan.Service.Game.Package;

internal sealed class PackageConvertStatus
{
    public PackageConvertStatus(string name)
    {
        Name = name;
    }

    public PackageConvertStatus(string name, long bytesRead, long totalBytes)
    {
        Percent = (double)bytesRead / totalBytes;
        Name = name;
        Description = $"{Converters.ToFileSizeString(bytesRead)}/{Converters.ToFileSizeString(totalBytes)}";
    }

    public string Name { get; }

    public string Description { get; } = default!;

    public double Percent { get; } = -1;

    public bool IsIndeterminate { get => Percent < 0; }
}