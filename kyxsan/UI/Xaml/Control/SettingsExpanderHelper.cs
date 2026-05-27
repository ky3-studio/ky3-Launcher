//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.Controls;
using Microsoft.UI.Xaml;

namespace kyxsan.UI.Xaml.Control;

[SuppressMessage("", "SH001")]
[DependencyProperty<bool>("IsItemsEnabled", DefaultValue = true, PropertyChangedCallbackName = nameof(OnIsItemsEnabledChanged), IsAttached = true, TargetType = typeof(SettingsExpander), NotNull = true)]
public sealed partial class SettingsExpanderHelper
{
    private static void OnIsItemsEnabledChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
    {
        foreach (object item in ((SettingsExpander)dp).Items)
        {
            if (item is Microsoft.UI.Xaml.Controls.Control control)
            {
                control.IsEnabled = (bool)e.NewValue;
            }
        }
    }
}