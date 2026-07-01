//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using System.Collections.ObjectModel;

namespace Launcher.Service.Notification;

[Service(ServiceLifetime.Singleton, typeof(IInfoBarService))]
internal sealed partial class InfoBarService : IInfoBarService, IRecipient<InfoBarMessage>
{
    private readonly ITaskContext taskContext;

    [GeneratedConstructor]
    public partial InfoBarService(IServiceProvider serviceProvider);

    [field: MaybeNull]
    public ObservableCollection<InfoBarOptions> Collection { get => LazyInitializer.EnsureInitialized(ref field, () => []); }

    public void Receive(InfoBarMessage message)
    {
#pragma warning disable CA2012
        PrivateShowAsync(InfoBarOptions.Create(message)).SafeForget();
#pragma warning restore CA2012
    }

    private async ValueTask PrivateShowAsync(InfoBarOptions infoBarOptions)
    {
        await taskContext.SwitchToMainThreadAsync();

        try
        {
            Collection.Insert(0, infoBarOptions);
        }
        catch
        {
            // Ignore
        }
    }
}