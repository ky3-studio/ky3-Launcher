//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Core.IO.Ini;
using kyxsan.Service.Game.FileSystem;
using kyxsan.Service.Game.Scheme;
using kyxsan.Win32;
using kyxsan.Win32.Foundation;
using System.Collections.Immutable;
using System.IO;
using System.Runtime.InteropServices;

namespace kyxsan.Service.Game.Configuration;

internal static class GameConfiguration
{
    public static ChannelOptions Read(IGameFileSystem gameFileSystem)
    {
        string configFilePath = gameFileSystem.GameConfigurationFilePath;
        ImmutableArray<IniElement> elements;
        try
        {
            elements = IniSerializer.DeserializeFromFile(configFilePath);
        }
        catch (IOException ex)
        {
            // The process cannot access the file '?' because it is being used by another process.
            if (kyxsanNative.IsWin32(ex.HResult, WIN32_ERROR.ERROR_SHARING_VIOLATION))
            {
                return ChannelOptions.SharingViolation(configFilePath);
            }

            if (kyxsanNative.IsWin32(ex.HResult, [WIN32_ERROR.ERROR_NOT_READY, WIN32_ERROR.ERROR_NO_SUCH_DEVICE]))
            {
                return ChannelOptions.DeviceNotFound(gameFileSystem.GameDirectory);
            }

            throw;
        }

        string? channel = default;
        string? subChannel = default;

        foreach (IniElement element in elements)
        {
            if (element is not IniParameter parameter)
            {
                continue;
            }

            switch (parameter.Key)
            {
                case ChannelOptions.ChannelName:
                    channel = parameter.Value;
                    break;
                case ChannelOptions.SubChannelName:
                    subChannel = parameter.Value;
                    break;
            }

            if (channel is not null && subChannel is not null)
            {
                break;
            }
        }

        return new(channel, subChannel, gameFileSystem.IsExecutableOversea);
    }

    public static bool Read(IGameFileSystem gameFileSystem, string parameterKey)
    {
        ImmutableArray<IniElement> elements;
        try
        {
            elements = IniSerializer.DeserializeFromFile(gameFileSystem.GameConfigurationFilePath);
        }
        catch (IOException ex)
        {
            if (kyxsanNative.IsWin32(ex.HResult, WIN32_ERROR.ERROR_NOT_READY))
            {
                return false;
            }

            throw;
        }

        return elements.Any(e => e is IniParameter parameter && string.Equals(parameter.Key, parameterKey, StringComparison.OrdinalIgnoreCase));
    }

    public static void Create(LaunchScheme launchScheme, string version, string configFilePath)
    {
        string gameBiz = launchScheme.IsOversea ? "hk4e_global" : "hk4e_cn";
        string content = $$$"""
            [general]
            uapc={"{{{gameBiz}}}":{"uapc":""},"hyp":{"uapc":""}}
            channel={{{launchScheme.Channel:D}}}
            sub_channel={{{launchScheme.SubChannel:D}}}
            cps=gw_pc
            game_version={{{version}}}
            """;

        string? directory = Path.GetDirectoryName(configFilePath);
        ArgumentNullException.ThrowIfNull(directory);
        Directory.CreateDirectory(directory);
        File.WriteAllText(configFilePath, content);
    }

    public static bool Patch(LaunchScheme launchScheme, string scriptVersionFilePath, string configFilePath)
    {
        if (!File.Exists(scriptVersionFilePath))
        {
            return false;
        }

        string version = File.ReadAllText(scriptVersionFilePath);
        Create(launchScheme, version, configFilePath);

        return true;
    }

    public static bool UpdateVersion(string configFilePath, string version)
    {
        bool updated = false;
        IniElement[]? ini = ImmutableCollectionsMarshal.AsArray(IniSerializer.DeserializeFromFile(configFilePath));

        if (ini is null)
        {
            return false;
        }

        foreach (ref IniElement element in ini.AsSpan())
        {
            if (element is not IniParameter { Key: "game_version" } parameter)
            {
                continue;
            }

            element = parameter.WithValue(version, out updated);
            break;
        }

        IniSerializer.SerializeToFile(configFilePath, ini);
        return updated;
    }
}