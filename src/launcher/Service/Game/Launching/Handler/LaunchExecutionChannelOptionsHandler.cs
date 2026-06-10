//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Core.ExceptionService;
using kyxsan.Core.IO.Ini;
using kyxsan.Service.Game.Configuration;
using kyxsan.Service.Game.FileSystem;
using kyxsan.Service.Game.Launching.Context;
using System.IO;
using System.Runtime.InteropServices;

namespace kyxsan.Service.Game.Launching.Handler;

internal sealed class LaunchExecutionChannelOptionsHandler : AbstractLaunchExecutionHandler
{
    public override ValueTask BeforeAsync(BeforeLaunchExecutionContext context)
    {
        string configPath = context.FileSystem.GameConfigurationFilePath;

        IniElement[]? elements;
        try
        {
            elements = ImmutableCollectionsMarshal.AsArray(IniSerializer.DeserializeFromFile(configPath));
            ArgumentNullException.ThrowIfNull(elements);
        }
        catch (FileNotFoundException fnfEx)
        {
            return ValueTask.FromException(kyxsanException.NotSupported(SH.FormatServiceGameSetMultiChannelConfigFileNotFound(configPath), fnfEx));
        }
        catch (DirectoryNotFoundException dnfEx)
        {
            return ValueTask.FromException(kyxsanException.NotSupported(SH.FormatServiceGameSetMultiChannelConfigFileNotFound(configPath), dnfEx));
        }
        catch (UnauthorizedAccessException uaEx)
        {
            return ValueTask.FromException(kyxsanException.NotSupported(SH.ServiceGameSetMultiChannelUnauthorizedAccess, uaEx));
        }

        IEnumerable<IniElement> toSerialize = UpdateChannelOptionsFromTargetLaunchScheme(elements, context);

        if (context.TryGetOption(LaunchExecutionOptionsKey.ChannelOptionsChanged, out bool changed) && changed)
        {
            IniSerializer.SerializeToFile(configPath, toSerialize);
        }

        context.ServiceProvider.GetRequiredService<IGameConfigurationFileService>()
            .Backup(context.FileSystem.GameConfigurationFilePath, context.FileSystem.IsExecutableOversea);

        return ValueTask.CompletedTask;
    }

    private static IEnumerable<IniElement> UpdateChannelOptionsFromTargetLaunchScheme(IniElement[] elements, BeforeLaunchExecutionContext context)
    {
        bool channelFound = false;
        bool subChannelFound = false;
        string targetChannel = context.TargetScheme.Channel.ToString("D");
        string targetSubChannel = context.TargetScheme.SubChannel.ToString("D");

        foreach (ref IniElement element in elements.AsSpan())
        {
            if (element is not IniParameter parameter)
            {
                continue;
            }

            switch (parameter.Key)
            {
                case ChannelOptions.ChannelName:
                    {
                        channelFound = true;
                        element = parameter.WithValue(targetChannel, out bool changed);
                        context.TryGetOption(LaunchExecutionOptionsKey.ChannelOptionsChanged, out bool previous);
                        context.SetOption(LaunchExecutionOptionsKey.ChannelOptionsChanged, changed || previous);
                        continue;
                    }

                case ChannelOptions.SubChannelName:
                    {
                        subChannelFound = true;
                        element = parameter.WithValue(targetSubChannel, out bool changed);
                        context.TryGetOption(LaunchExecutionOptionsKey.ChannelOptionsChanged, out bool previous);
                        context.SetOption(LaunchExecutionOptionsKey.ChannelOptionsChanged, changed || previous);
                        continue;
                    }
            }
        }

        if (channelFound && subChannelFound)
        {
            return elements;
        }

        context.SetOption(LaunchExecutionOptionsKey.ChannelOptionsChanged, true);
        List<IniElement> result = [.. elements];

        if (!channelFound)
        {
            result.Add(new IniParameter(ChannelOptions.ChannelName, targetChannel));
        }

        if (!subChannelFound)
        {
            result.Add(new IniParameter(ChannelOptions.SubChannelName, targetSubChannel));
        }

        return result;
    }
}