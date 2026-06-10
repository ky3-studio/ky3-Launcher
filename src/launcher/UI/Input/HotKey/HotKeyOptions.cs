//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using kyxsan.Core.Setting;
using kyxsan.Model;
using kyxsan.Service.Game;
using kyxsan.Win32;
using kyxsan.Win32.Foundation;
using kyxsan.Win32.UI.Input.KeyboardAndMouse;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace kyxsan.UI.Input.HotKey;

[Service(ServiceLifetime.Singleton)]
internal sealed partial class HotKeyOptions : ObservableObject, IDisposable
{
    private readonly IServiceProvider serviceProvider;
    private readonly ITaskContext taskContext;

    private bool isDisposed;

    [GeneratedConstructor]
    public partial HotKeyOptions(IServiceProvider serviceProvider);

    static unsafe HotKeyOptions()
    {
        kyxsanNativeHotKeyAction.InitializeBeforeSwitchCallback(kyxsanNativeHotKeyBeforeSwitchCallback.Create(&HandleShouldPreventSwitch));
    }

    public static bool IsInGameOnly
    {
        get => LocalSetting.Get(SettingKeys.HotKeyRepeatForeverInGameOnly, false);
        set => LocalSetting.Set(SettingKeys.HotKeyRepeatForeverInGameOnly, value);
    }

    public static ImmutableArray<NameValue<VIRTUAL_KEY>> VirtualKeys { get => Input.VirtualKeys.HotKeyValues; }

    [ObservableProperty]
    public partial HotKeyCombination? MouseClickRepeatForeverKeyCombination { get; set; }

    [ObservableProperty]
    public partial HotKeyCombination? KeyPressRepeatForeverKeyCombination { get; set; }

    public async ValueTask InitializeAsync()
    {
        await taskContext.SwitchToMainThreadAsync();

        MouseClickRepeatForeverKeyCombination = new(serviceProvider, kyxsanNativeHotKeyActionKind.MouseClickRepeatForever, SettingKeys.HotKeyMouseClickRepeatForever);
        KeyPressRepeatForeverKeyCombination = new(serviceProvider, kyxsanNativeHotKeyActionKind.KeyPressRepeatForever, SettingKeys.HotKeyKeyPressRepeatForever);

        MouseClickRepeatForeverKeyCombination.Initialize();
        KeyPressRepeatForeverKeyCombination.Initialize();

        GameLifeCycle.IsGameRunningProperty.PropertyChanged += OnGameRunningChanged;
    }

    public void Dispose()
    {
        if (Interlocked.Exchange(ref isDisposed, true))
        {
            return;
        }

        GameLifeCycle.IsGameRunningProperty.PropertyChanged -= OnGameRunningChanged;
        MouseClickRepeatForeverKeyCombination?.Dispose();
        KeyPressRepeatForeverKeyCombination?.Dispose();
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static BOOL HandleShouldPreventSwitch()
    {
        return IsInGameOnly && !GameLifeCycle.IsGameRunningRequiresMainThread();
    }

    private void OnGameRunningChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is not "Value" || !IsInGameOnly)
        {
            return;
        }

        if (GameLifeCycle.IsGameRunningProperty.Value)
        {
            MouseClickRepeatForeverKeyCombination?.RestoreEnabled();
            KeyPressRepeatForeverKeyCombination?.RestoreEnabled();
        }
        else
        {
            MouseClickRepeatForeverKeyCombination?.SetNativeEnabled(false);
            KeyPressRepeatForeverKeyCombination?.SetNativeEnabled(false);
        }
    }
}