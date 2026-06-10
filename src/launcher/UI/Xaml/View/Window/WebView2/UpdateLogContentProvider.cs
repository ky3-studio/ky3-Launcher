//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.Web.WebView2.Core;
using kyxsan.Service;
using Windows.Graphics;
using Windows.System;

namespace kyxsan.UI.Xaml.View.Window.WebView2;

internal sealed class UpdateLogContentProvider : IWebView2ContentProvider
{
    private string? languageCode;

    public ElementTheme ActualTheme { get; set; }

    public CoreWebView2? CoreWebView2 { get; set; }

    public Action? CloseWindowAction { get; set; }

    public ValueTask InitializeAsync(IServiceProvider serviceProvider, CancellationToken token)
    {
        languageCode = serviceProvider.GetRequiredService<CultureOptions>().LanguageCode;

        ArgumentNullException.ThrowIfNull(CoreWebView2);
        CoreWebView2.AddWebResourceRequestedFilter("about:blank", CoreWebView2WebResourceContext.Document);
        CoreWebView2.NewWindowRequested += OnNewWindowRequested;
        CoreWebView2.WebResourceRequested += OnWebResourceRequested;
        CoreWebView2.Navigate("about:blank");
        return ValueTask.CompletedTask;
    }

    public RectInt32 InitializePosition(RectInt32 parentRect, double parentDpi)
    {
        return WebView2WindowPosition.Vertical(parentRect, parentDpi);
    }

    public void Unload()
    {
        if (CoreWebView2 is not null)
        {
            CoreWebView2.NewWindowRequested -= OnNewWindowRequested;
            CoreWebView2.WebResourceRequested -= OnWebResourceRequested;
        }
    }

    private void OnNewWindowRequested(object? sender, CoreWebView2NewWindowRequestedEventArgs e)
    {
        e.Handled = true;
        _ = Launcher.LaunchUriAsync(e.Uri.ToUri());
    }

    private void OnWebResourceRequested(CoreWebView2 coreWebView2, CoreWebView2WebResourceRequestedEventArgs args)
    {
        args.Request.Headers.SetHeader("Accept-Language", languageCode);
    }
}