//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Launcher.Web.Endpoint.Launcher;
using System.Net.Http;

namespace Launcher.ViewModel.Wiki;

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

 
    private const int BatchSize = 50;

  
    private const int StopAfterGap = 40;

    protected override async ValueTask<bool> LoadOverrideAsync(CancellationToken token)
    {
        List<int> found = [];
        int maxHit = 0;
        int start = 1;

        while (true)
        {
            IEnumerable<Task<int>> checkTasks = Enumerable.Range(start, BatchSize).Select(async i =>
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

            foreach (int i in results.Where(i => i > 0))
            {
                found.Add(i);
                if (i > maxHit)
                {
                    maxHit = i;
                }
            }

            int probedTo = start + BatchSize - 1;
            if (probedTo - maxHit >= StopAfterGap)
            {
                break;
            }

            start += BatchSize;
        }

        List<EmotionIconEntry> icons = found
            .OrderBy(i => i)
            .Select(i => new EmotionIconEntry(i, new Uri(StaticResourcesEndpoints.StaticRaw("EmotionIcon", $"UI_EmotionIcon{i}.png"))))
            .ToList();

        await taskContext.SwitchToMainThreadAsync();
        EmotionIcons = icons;
        return true;
    }
}
