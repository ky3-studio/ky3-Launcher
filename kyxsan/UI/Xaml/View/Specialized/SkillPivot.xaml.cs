//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections;

namespace kyxsan.UI.Xaml.View.Specialized;

[DependencyProperty<IList>("Skills", PropertyChangedCallbackName = nameof(OnSkillsChanged))]
[DependencyProperty<object>("Selected")]
[DependencyProperty<DataTemplate>("ItemTemplate")]
internal sealed partial class SkillPivot : UserControl
{
    public SkillPivot()
    {
        InitializeComponent();
    }

    private static void OnSkillsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        if (sender is SkillPivot skillPivot)
        {
            if (args.OldValue != args.NewValue && args.NewValue as IList is [{ } target, ..])
            {
                skillPivot.Bindings.Update();
                skillPivot.Selected = target;
            }
        }
    }
}