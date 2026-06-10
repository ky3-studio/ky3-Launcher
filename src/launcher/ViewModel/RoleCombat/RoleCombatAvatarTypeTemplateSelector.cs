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
using kyxsan.Web.Hoyolab.Takumi.GameRecord.RoleCombat;

namespace kyxsan.ViewModel.RoleCombat;

internal sealed partial class RoleCombatAvatarTypeTemplateSelector : DataTemplateSelector
{
    private static readonly DataTemplate EmptyDataTemplate = new();

    public DataTemplate? TrialTemplate { get; set; }

    public DataTemplate? SupportTemplate { get; set; }

    protected override DataTemplate? SelectTemplateCore(object item, DependencyObject container)
    {
        if (item is RoleCombatAvatarType type)
        {
            return type switch
            {
                RoleCombatAvatarType.Trial => TrialTemplate,
                RoleCombatAvatarType.Support => SupportTemplate,
                _ => EmptyDataTemplate,
            };
        }

        return base.SelectTemplateCore(item, container);
    }
}