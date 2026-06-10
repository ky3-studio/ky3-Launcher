//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Service.Navigation;

namespace kyxsan.ViewModel.LauncherHome;

[BindableCustomPropertyProvider]
[Service(ServiceLifetime.Scoped)]
internal sealed partial class LauncherHomeViewModel : Abstraction.ViewModel
{
    private readonly INavigationService navigationService;

    [GeneratedConstructor]
    public partial LauncherHomeViewModel(IServiceProvider serviceProvider);

    protected override ValueTask<bool> LoadOverrideAsync(CancellationToken token)
    {
        return ValueTask.FromResult(true);
    }

    [Command("LaunchGameCommand")]
    private void NavigateToLaunchGame()
    {
        navigationService.Navigate<UI.Xaml.View.Page.LaunchGamePage>(NavigationExtraData.Default);
    }
}
