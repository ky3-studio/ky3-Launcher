//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Web.Endpoint.kyxsan;
using System.Net.Http;

namespace kyxsan.ViewModel.Wiki;

internal sealed record EmotionIconEntry(int Number, Uri ImageUri);

[BindableCustomPropertyProvider]
[Service(ServiceLifetime.Scoped)]
internal sealed partial class EmotionIconViewModel : Abstraction.ViewModel
{
    private readonly ITaskContext taskContext;
    private readonly IHttpClientFactory httpClientFactory;

    [GeneratedConstructor]
    public partial EmotionIconViewModel(IServiceProvider serviceProvider);

    public IReadOnlyList<EmotionIconEntry>? EmotionIcons { get; private set => SetProperty(ref field, value); }

    protected override async ValueTask<bool> LoadOverrideAsync(CancellationToken token)
    {
        IEnumerable<Task<int>> checkTasks = Enumerable.Range(1, 760).Select(async i =>
        {
            try
            {
                using HttpClient client = httpClientFactory.CreateClient();
                string url = StaticResourcesEndpoints.StaticRaw("EmotionIcon", $"UI_EmotionIcon{i}.png");
                using HttpRequestMessage request = new(HttpMethod.Head, url);
                using HttpResponseMessage response = await client.SendAsync(request, token).ConfigureAwait(false);
                return response.IsSuccessStatusCode ? i : -1;
            }
            catch
            {
                return -1;
            }
        });

        int[] results = await Task.WhenAll(checkTasks).ConfigureAwait(false);

        List<EmotionIconEntry> icons = results
            .Where(i => i > 0)
            .OrderBy(i => i)
            .Select(i => new EmotionIconEntry(i, new Uri(StaticResourcesEndpoints.StaticRaw("EmotionIcon", $"UI_EmotionIcon{i}.png"))))
            .ToList();

        await taskContext.SwitchToMainThreadAsync();
        EmotionIcons = icons;
        return true;
    }
}
