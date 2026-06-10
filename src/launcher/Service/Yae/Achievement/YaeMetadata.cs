//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___          __   __ _    _____
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \  __  __ \ \ / // \  | ____|
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | | \ \/ /  \ V // _ \ |  _|
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |  >  <    | |/ ___ \| |___
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/  /_/\_\   |_/_/   \_\_____|
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using Google.Protobuf;
using kyxsan.Core.Protobuf;

namespace kyxsan.Service.Yae.Achievement;

internal sealed class YaeMethodRvaConfig
{
    public uint DoCmd { get; set; }

    public uint UpdateNormalProp { get; set; }

    public uint NewString { get; set; }

    public uint FindGameObject { get; set; }

    public uint EventSystemUpdate { get; set; }

    public uint SimulatePointerClick { get; set; }

    public uint ToInt32 { get; set; }

    public uint TcpStatePtr { get; set; }

    public uint SharedInfoPtr { get; set; }

    public uint Decompress { get; set; }

    public static YaeMethodRvaConfig Parse(CodedInputStream stream)
    {
        YaeMethodRvaConfig config = new();
        while (stream.TryReadTag(out uint tag))
        {
            switch (WireFormat.GetTagFieldNumber(tag))
            {
                case 1: config.DoCmd = stream.ReadUInt32(); break;
                case 3: config.UpdateNormalProp = stream.ReadUInt32(); break;
                case 4: config.NewString = stream.ReadUInt32(); break;
                case 5: config.FindGameObject = stream.ReadUInt32(); break;
                case 6: config.EventSystemUpdate = stream.ReadUInt32(); break;
                case 7: config.SimulatePointerClick = stream.ReadUInt32(); break;
                case 8: config.ToInt32 = stream.ReadUInt32(); break;
                case 9: config.TcpStatePtr = stream.ReadUInt32(); break;
                case 10: config.SharedInfoPtr = stream.ReadUInt32(); break;
                case 11: config.Decompress = stream.ReadUInt32(); break;
                default: stream.SkipLastField(); break;
            }
        }

        return config;
    }
}

internal sealed class YaeNativeLibConfig
{
    public uint StoreCmdId { get; set; }

    public uint AchievementCmdId { get; set; }

    public Dictionary<uint, YaeMethodRvaConfig> MethodRva { get; } = [];

    public static YaeNativeLibConfig Parse(CodedInputStream stream)
    {
        YaeNativeLibConfig config = new();
        while (stream.TryReadTag(out uint tag))
        {
            switch (WireFormat.GetTagFieldNumber(tag))
            {
                case 1: config.StoreCmdId = stream.ReadUInt32(); break;
                case 2: config.AchievementCmdId = stream.ReadUInt32(); break;
                case 10:
                    {
                        using CodedInputStream entryStream = stream.UnsafeReadLengthDelimitedStream();
                        uint key = 0;
                        YaeMethodRvaConfig? value = null;
                        while (entryStream.TryReadTag(out uint entryTag))
                        {
                            switch (WireFormat.GetTagFieldNumber(entryTag))
                            {
                                case 1: key = entryStream.ReadUInt32(); break;
                                case 2:
                                    {
                                        using CodedInputStream valueStream = entryStream.UnsafeReadLengthDelimitedStream();
                                        value = YaeMethodRvaConfig.Parse(valueStream);
                                        break;
                                    }

                                default: entryStream.SkipLastField(); break;
                            }
                        }

                        if (value is not null)
                        {
                            config.MethodRva[key] = value;
                        }

                        break;
                    }

                default: stream.SkipLastField(); break;
            }
        }

        return config;
    }
}

internal sealed class YaeAchievementProtoFieldInfo
{
    public uint Id { get; set; }

    public uint Status { get; set; }

    public uint TotalProgress { get; set; }

    public uint CurrentProgress { get; set; }

    public uint FinishTimestamp { get; set; }

    public static YaeAchievementProtoFieldInfo Parse(CodedInputStream stream)
    {
        YaeAchievementProtoFieldInfo info = new();
        while (stream.TryReadTag(out uint tag))
        {
            switch (WireFormat.GetTagFieldNumber(tag))
            {
                case 1: info.Id = stream.ReadUInt32(); break;
                case 2: info.Status = stream.ReadUInt32(); break;
                case 3: info.TotalProgress = stream.ReadUInt32(); break;
                case 4: info.CurrentProgress = stream.ReadUInt32(); break;
                case 5: info.FinishTimestamp = stream.ReadUInt32(); break;
                default: stream.SkipLastField(); break;
            }
        }

        return info;
    }
}

internal sealed class YaeAchievementInfo
{
    public string Version { get; set; } = string.Empty;

    public YaeAchievementProtoFieldInfo? PbInfo { get; set; }

    public YaeNativeLibConfig? NativeConfig { get; set; }

    public static YaeAchievementInfo Parse(byte[] data)
    {
        using CodedInputStream stream = new(data);
        YaeAchievementInfo info = new();
        while (stream.TryReadTag(out uint tag))
        {
            switch (WireFormat.GetTagFieldNumber(tag))
            {
                case 1: info.Version = stream.ReadString(); break;
                case 4:
                    {
                        using CodedInputStream subStream = stream.UnsafeReadLengthDelimitedStream();
                        info.PbInfo = YaeAchievementProtoFieldInfo.Parse(subStream);
                        break;
                    }

                case 5:
                    {
                        using CodedInputStream subStream = stream.UnsafeReadLengthDelimitedStream();
                        info.NativeConfig = YaeNativeLibConfig.Parse(subStream);
                        break;
                    }

                default: stream.SkipLastField(); break;
            }
        }

        return info;
    }
}
