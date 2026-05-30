//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using Microsoft.Web.WebView2.Core;
using System.IO;

namespace kyxsan.Web.WebView2;

internal static class CoreWebView2EnvironmentFactory
{
    private static Task<CoreWebView2Environment>? environmentTask;

    public static Task<CoreWebView2Environment> GetAsync()
    {
        if (environmentTask is { IsFaulted: true } or { IsCanceled: true })
        {
            environmentTask = null;
        }

        return environmentTask ??= CreateAsync();
    }

    private static async Task<CoreWebView2Environment> CreateAsync()
    {
        CoreWebView2EnvironmentOptions options = new()
        {
            AdditionalBrowserArguments = "--do-not-de-elevate --autoplay-policy=no-user-gesture-required",
        };

        string? userDataFolder = null;
        if (!global::kyxsan.Core.ApplicationModel.PackageIdentityAdapter.HasPackageIdentity)
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            userDataFolder = Path.Combine(appData, "CustomWebView2");
        }

        return await CoreWebView2Environment.CreateWithOptionsAsync(null, userDataFolder, options);
    }
}
