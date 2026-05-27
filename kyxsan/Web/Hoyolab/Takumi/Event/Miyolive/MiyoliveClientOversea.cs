//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Core.DependencyInjection.Annotation.HttpClient;
using kyxsan.Core.ExceptionService;
using kyxsan.Web.Response;
using System.Net.Http;

namespace kyxsan.Web.Hoyolab.Takumi.Event.Miyolive;

[HttpClient(HttpClientConfiguration.Default)]
internal sealed partial class MiyoliveClientOversea : IMiyoliveClient
{
    [GeneratedConstructor]
    public partial MiyoliveClientOversea(IServiceProvider serviceProvider, HttpClient httpClient);

    public ValueTask<Response<CodeListWrapper>> RefreshCodeAsync(string actId, CancellationToken token = default)
    {
        return ValueTask.FromException<Response<CodeListWrapper>>(kyxsanException.NotSupported());
    }
}