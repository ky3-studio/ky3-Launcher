//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using kyxsan.Service.Navigation.Message;

namespace kyxsan.Service.Navigation;

[Service(ServiceLifetime.Singleton, typeof(INavigationService))]
internal sealed partial class NavigationService : INavigationService
{
    private readonly ITaskContext taskContext;
    private readonly IMessenger messenger;

    [GeneratedConstructor]
    public partial NavigationService(IServiceProvider serviceProvider);

    public NavigationResult Navigate(Type pageType, INavigationCompletionSource data, bool syncNavigationViewItem = false)
    {
        NavigationNavigateMessage message = new()
        {
            PageType = pageType,
            Data = data,
            SyncNavigationViewItem = syncNavigationViewItem,
        };

        messenger.Send(message);
        return message.Result;
    }

    public NavigationResult Navigate<TPage>(INavigationCompletionSource data, bool syncNavigationViewItem = false)
        where TPage : Page
    {
        return Navigate(typeof(TPage), data, syncNavigationViewItem);
    }

    public async ValueTask<NavigationResult> NavigateAsync<TPage>(INavigationCompletionSource data, bool syncNavigationViewItem = false)
        where TPage : Page
    {
        await taskContext.SwitchToMainThreadAsync();
        NavigationResult result = Navigate<TPage>(data, syncNavigationViewItem);

        if (result is NavigationResult.Succeed)
        {
            try
            {
                await taskContext.SwitchToBackgroundAsync();
                await data.WaitForCompletionAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                return NavigationResult.Failed;
            }
        }

        return result;
    }
}