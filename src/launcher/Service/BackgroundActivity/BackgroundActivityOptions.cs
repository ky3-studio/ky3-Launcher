//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;

namespace kyxsan.Service.BackgroundActivity;

[Service(ServiceLifetime.Singleton)]
internal sealed partial class BackgroundActivityOptions : ObservableObject
{
    [GeneratedConstructor]
    public partial BackgroundActivityOptions(IServiceProvider serviceProvider);

    public BackgroundActivity Default { get; } = new(string.Empty, string.Empty);

    public BackgroundActivity MetadataInitialization { get; } = new(SH.ServiceBackgroundActivityMetadataInitialization, SH.ServiceBackgroundActivityMetadataInitializationDescription);

    public BackgroundActivity FullTrustInitialization { get; } = new(SH.ServiceBackgroundActivityFullTrustInit, SH.ServiceBackgroundActivityFullTrustInitDescription);
}