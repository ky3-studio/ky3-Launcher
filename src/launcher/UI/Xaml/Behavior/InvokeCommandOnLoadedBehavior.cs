//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.Behaviors;
using JetBrains.Annotations;
using Microsoft.UI.Xaml;
using kyxsan.UI.Input;

namespace kyxsan.UI.Xaml.Behavior;

[UsedImplicitly]
[DependencyProperty<ICommand>("Command")]
[DependencyProperty<object>("CommandParameter")]
internal sealed partial class InvokeCommandOnLoadedBehavior : BehaviorBase<UIElement>
{
    private bool executed;

    protected override void OnAttached()
    {
        base.OnAttached();

        // FrameworkElement in a ItemsRepeater gets attached twice
        if (AssociatedObject is FrameworkElement { IsLoaded: true })
        {
            TryExecuteCommand();
        }
    }

    protected override void OnAssociatedObjectLoaded()
    {
        TryExecuteCommand();
    }

    private void TryExecuteCommand()
    {
        if (AssociatedObject is null)
        {
            return;
        }

        if (executed)
        {
            return;
        }

        executed = Command.TryExecute(CommandParameter);
    }
}