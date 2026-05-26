//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Model.Intrinsic;

namespace kyxsan.Service.Game.Configuration;

internal readonly struct ChannelOptions
{
    public const string ChannelName = "channel";
    public const string SubChannelName = "sub_channel";

    public readonly ChannelType Channel;

    public readonly SubChannelType SubChannel;

    public readonly bool IsOversea;

    public readonly ChannelOptionsErrorKind ErrorKind;

    public readonly string? FilePath;

    public ChannelOptions(string? channel, string? subChannel, bool isOversea)
    {
        _ = Enum.TryParse(channel, out Channel);
        _ = Enum.TryParse(subChannel, out SubChannel);
        IsOversea = isOversea;
    }

    private ChannelOptions(ChannelOptionsErrorKind errorKind, string? filePath)
    {
        ErrorKind = errorKind;
        FilePath = filePath;
    }

    public static ChannelOptions GamePathLocked(string filePath)
    {
        return new(ChannelOptionsErrorKind.GamePathLocked, filePath);
    }

    public static ChannelOptions ConfigurationFileNotFound(string filePath)
    {
        return new(ChannelOptionsErrorKind.ConfigurationFileNotFound, filePath);
    }

    public static ChannelOptions GamePathNullOrEmpty()
    {
        return new(ChannelOptionsErrorKind.GamePathNullOrEmpty, string.Empty);
    }

    public static ChannelOptions DeviceNotFound(string directory)
    {
        return new(ChannelOptionsErrorKind.DeviceNotFound, directory);
    }

    public static ChannelOptions GameContentCorrupted(string directory)
    {
        return new(ChannelOptionsErrorKind.GameContentCorrupted, directory);
    }

    public static ChannelOptions SharingViolation(string filePath)
    {
        return new(ChannelOptionsErrorKind.SharingViolation, filePath);
    }

    public override string ToString()
    {
        return $$"""
            { Channel: {{Channel}}, SubChannel: {{SubChannel}}, IsOversea: {{IsOversea}} }
            """;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Channel, SubChannel, IsOversea);
    }
}