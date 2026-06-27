//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Launcher.Core.Diagnostics;
using Launcher.Core.Property;
using Launcher.Factory.Process;
using Launcher.Win32;

namespace Launcher.Service.Game;

internal static class GameLifeCycle
{
    public static IObservableProperty<bool> IsGameRunningProperty { get; } = Property.CreateObservable(PrivateIsGameRunning(out _));

    public static IObservableProperty<bool> IsIslandConnected { get; } = Property.CreateObservable(false);

    public static async ValueTask SpinWaitGameExitAsync(ITaskContext taskContext)
    {
        await taskContext.SwitchToBackgroundAsync();
        unsafe
        {
            SpinWaitPolyfill.SpinWhile(&PrivateIsGameRunning);
        }

        await taskContext.SwitchToMainThreadAsync();
        IsGameRunningProperty.Value = false;
    }

    public static bool IsGameRunningRequiresMainThread()
    {
        bool result = PrivateIsGameRunning(out _);
        IsGameRunningProperty.Value = result;
        return result;
    }

    public static async ValueTask<bool> IsGameRunningAsync(ITaskContext taskContext)
    {
        await taskContext.SwitchToBackgroundAsync();
        bool result = PrivateIsGameRunning(out _);

        await taskContext.SwitchToMainThreadAsync();
        IsGameRunningProperty.Value = result;
        return result;
    }

    public static async ValueTask<ValueResult<bool, IProcess?>> TryGetRunningGameProcessAsync(ITaskContext taskContext)
    {
        await taskContext.SwitchToBackgroundAsync();
        bool result = PrivateIsGameRunning(out IProcess? runningProcess);

        await taskContext.SwitchToMainThreadAsync();
        IsGameRunningProperty.Value = result;
        return new(result, runningProcess);
    }

    public static async ValueTask<bool> TryKillGameProcessAsync(ITaskContext taskContext)
    {
        if (await TryGetRunningGameProcessAsync(taskContext).ConfigureAwait(false) is not (true, { } process))
        {
            return false;
        }

        try
        {
            process.Kill();

            await taskContext.SwitchToMainThreadAsync();
            IsGameRunningProperty.Value = PrivateIsGameRunning();
            return true;
        }
        catch (Exception ex)
        {
            LauncherNative.Instance.ShowErrorMessage(SH.ServiceGameLaunchExecutionGameRunningKillFailed, ex.Message);
            return false;
        }
    }

    private static bool PrivateIsGameRunning()
    {
        return ProcessFactory.IsRunning([GameConstants.YuanShenProcessName, GameConstants.GenshinImpactProcessName], out _);
    }

    private static bool PrivateIsGameRunning([NotNullWhen(true)] out IProcess? runningProcess)
    {
        return ProcessFactory.IsRunning([GameConstants.YuanShenProcessName, GameConstants.GenshinImpactProcessName], out runningProcess);
    }
}