//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using JetBrains.Annotations;
using kyxsan.Core.Setting;
using kyxsan.Model;
using kyxsan.Service.Notification;
using kyxsan.Win32;
using kyxsan.Win32.Foundation;
using kyxsan.Win32.UI.Input.KeyboardAndMouse;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace kyxsan.UI.Input.HotKey;

internal sealed partial class HotKeyCombination : ObservableObject, IDisposable
{
    private readonly IMessenger messenger;
    private readonly kyxsanNativeHotKeyActionKind kind;
    private readonly string settingKey;
    private GCHandle<HotKeyCombination> handle;

    private kyxsanNativeHotKeyAction? native;

    public HotKeyCombination(IServiceProvider serviceProvider, kyxsanNativeHotKeyActionKind kind, string settingKey)
    {
        messenger = serviceProvider.GetRequiredService<IMessenger>();
        this.kind = kind;
        this.settingKey = settingKey;

        // Initialize Property backing fields
        {
            HotKeyParameter parameter = HotKeyParameter.Default;
            long value = LocalSetting.Get(settingKey, Unsafe.As<HotKeyParameter, long>(ref parameter));
            HotKeyParameter actual = Unsafe.As<long, HotKeyParameter>(ref value);

            // HOT_KEY_MODIFIERS.MOD_WIN is reserved for use by the OS.
            // This line should keep exists, we allow user to set it long time ago.
            FieldRefOfModifiers(this) = actual.Modifiers & ~HOT_KEY_MODIFIERS.MOD_WIN;
            FieldRefOfModifierHasControl(this) = Modifiers.HasFlag(HOT_KEY_MODIFIERS.MOD_CONTROL);
            FieldRefOfModifierHasShift(this) = Modifiers.HasFlag(HOT_KEY_MODIFIERS.MOD_SHIFT);
            FieldRefOfModifierHasAlt(this) = Modifiers.HasFlag(HOT_KEY_MODIFIERS.MOD_ALT);

            FieldRefOfKeyNameValue(this) = VirtualKeys.HotKeyValues.SingleOrDefault(nk => nk.Value == actual.Key) ?? VirtualKeys.HotKeyValues.Last();
            FieldRefOfKey(this) = KeyNameValue!.Value;
        }

        handle = new(this);
    }

    [FieldAccessor]
    public bool ModifierHasControl { get; set => _ = SetProperty(ref field, value) && UpdateModifiers(); }

    [FieldAccessor]
    public bool ModifierHasShift { get; set => _ = SetProperty(ref field, value) && UpdateModifiers(); }

    [FieldAccessor]
    public bool ModifierHasAlt { get; set => _ = SetProperty(ref field, value) && UpdateModifiers(); }

    [AllowNull]
    [FieldAccessor]
    public NameValue<VIRTUAL_KEY> KeyNameValue
    {
        get;
        set
        {
            if (value is not null && SetProperty(ref field, value))
            {
                Key = value.Value;
            }
        }
    }

    [FieldAccessor]
    public HOT_KEY_MODIFIERS Modifiers
    {
        get;
        private set
        {
            if (SetProperty(ref field, value))
            {
                OnPropertyChanged(nameof(DisplayName));
                SaveAndUpdate();
            }
        }
    }

    [FieldAccessor]
    public VIRTUAL_KEY Key
    {
        get;
        private set
        {
            if (SetProperty(ref field, value))
            {
                OnPropertyChanged(nameof(DisplayName));
                SaveAndUpdate();
            }
        }
    }

    public int ModifierIndex
    {
        get
        {
            if (Modifiers.HasFlag(HOT_KEY_MODIFIERS.MOD_CONTROL)) return 1;
            if (Modifiers.HasFlag(HOT_KEY_MODIFIERS.MOD_ALT)) return 2;
            if (Modifiers.HasFlag(HOT_KEY_MODIFIERS.MOD_SHIFT)) return 3;
            return 0;
        }

        set
        {
            HOT_KEY_MODIFIERS newMod = value switch
            {
                1 => HOT_KEY_MODIFIERS.MOD_CONTROL,
                2 => HOT_KEY_MODIFIERS.MOD_ALT,
                3 => HOT_KEY_MODIFIERS.MOD_SHIFT,
                _ => default,
            };

            FieldRefOfModifierHasControl(this) = newMod.HasFlag(HOT_KEY_MODIFIERS.MOD_CONTROL);
            FieldRefOfModifierHasShift(this) = newMod.HasFlag(HOT_KEY_MODIFIERS.MOD_SHIFT);
            FieldRefOfModifierHasAlt(this) = newMod.HasFlag(HOT_KEY_MODIFIERS.MOD_ALT);
            Modifiers = newMod;
        }
    }

    public int KeyCode
    {
        get => (int)Key;

        set
        {
            VIRTUAL_KEY newKey = (VIRTUAL_KEY)value;
            FieldRefOfKeyNameValue(this) = VirtualKeys.HotKeyValues.SingleOrDefault(nk => nk.Value == newKey) ?? VirtualKeys.HotKeyValues.Last();
            Key = newKey;
        }
    }

    /// <summary>
    /// Can perform the action.
    /// </summary>
    public bool IsEnabled
    {
        get => native?.IsEnabled ?? false;
        set
        {
            if (native is not null)
            {
                native.IsEnabled = value;
                OnPropertyChanged();
            }

            LocalSetting.Set($"{settingKey}.IsEnabled", value);
        }
    }

    /// <summary>
    /// Is performing the action.
    /// </summary>
    [ObservableProperty]
    [UsedImplicitly]
    public partial bool IsOn { get; private set; }

    public string DisplayName { get => ToString(); }

    public unsafe void Initialize()
    {
        native = kyxsanNative.Instance.MakeHotKeyAction(kind, kyxsanNativeHotKeyActionCallback.Create(&OnAction), handle);
        native.IsEnabled = LocalSetting.Get($"{settingKey}.IsEnabled", false);
        SaveAndUpdate();
    }

    public void Dispose()
    {
        handle.Dispose();
        native = default;
    }

    public void SetNativeEnabled(bool enabled)
    {
        if (native is not null)
        {
            native.IsEnabled = enabled;
        }
    }

    public void RestoreEnabled()
    {
        if (native is not null)
        {
            native.IsEnabled = LocalSetting.Get($"{settingKey}.IsEnabled", false);
        }
    }

    public override string ToString()
    {
        StringBuilder stringBuilder = new();

        if (Modifiers.HasFlag(HOT_KEY_MODIFIERS.MOD_CONTROL))
        {
            stringBuilder.Append("Ctrl").Append(" + ");
        }

        if (Modifiers.HasFlag(HOT_KEY_MODIFIERS.MOD_SHIFT))
        {
            stringBuilder.Append("Shift").Append(" + ");
        }

        if (Modifiers.HasFlag(HOT_KEY_MODIFIERS.MOD_ALT))
        {
            stringBuilder.Append("Alt").Append(" + ");
        }

        stringBuilder.Append(Key.ToString().AsSpan()[3..].Trim('_'));

        return stringBuilder.ToString();
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static void OnAction(BOOL isOn, GCHandle<HotKeyCombination> data)
    {
        if (data.Target is not { } combination)
        {
            return;
        }

        combination.IsOn = isOn;
    }

    private bool UpdateModifiers()
    {
        HOT_KEY_MODIFIERS modifiers = default;

        if (ModifierHasControl)
        {
            modifiers |= HOT_KEY_MODIFIERS.MOD_CONTROL;
        }

        if (ModifierHasShift)
        {
            modifiers |= HOT_KEY_MODIFIERS.MOD_SHIFT;
        }

        if (ModifierHasAlt)
        {
            modifiers |= HOT_KEY_MODIFIERS.MOD_ALT;
        }

        Modifiers = modifiers;
        return true;
    }

    private unsafe void SaveAndUpdate()
    {
        HotKeyParameter current = new(Modifiers, Key);
        LocalSetting.Set(settingKey, *(long*)&current);

        try
        {
            native?.Update(Modifiers, (uint)Key);
        }
        catch (Exception ex)
        {
            if (kyxsanNative.IsWin32(ex.HResult, WIN32_ERROR.ERROR_HOTKEY_ALREADY_REGISTERED))
            {
                messenger.Send(InfoBarMessage.Warning(SH.FormatCoreWindowHotkeyCombinationRegisterFailed(kind, DisplayName)));
            }
            else
            {
                messenger.Send(InfoBarMessage.Error(ex));
            }
        }
    }
}