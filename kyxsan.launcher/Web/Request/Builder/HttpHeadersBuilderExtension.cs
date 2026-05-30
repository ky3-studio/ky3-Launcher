//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Core.Abstraction;
using kyxsan.Web.Request.Builder.Abstraction;
using System.Diagnostics;
using System.Net.Http.Headers;

namespace kyxsan.Web.Request.Builder;

internal static class HttpHeadersBuilderExtension
{
    [DebuggerStepThrough]
    public static T AddHeader<T>(this T builder, string name)
        where T : class, IHttpHeadersBuilder<HttpHeaders>
    {
        return builder.AddHeader<T, HttpHeaders>(name, value: null);
    }

    [DebuggerStepThrough]
    public static TBuilder AddHeader<TBuilder, THeaders>(this TBuilder builder, string name)
        where TBuilder : class, IHttpHeadersBuilder<THeaders>
        where THeaders : HttpHeaders
    {
        return builder.AddHeader<TBuilder, THeaders>(name, value: null);
    }

    [DebuggerStepThrough]
    public static T AddHeader<T>(this T builder, string name, string? value)
        where T : class, IHttpHeadersBuilder<HttpHeaders>
    {
        return builder.AddHeader<T, HttpHeaders>(name, value);
    }

    [DebuggerStepThrough]
    public static TBuilder AddHeader<TBuilder, THeaders>(this TBuilder builder, string name, string? value)
        where TBuilder : class, IHttpHeadersBuilder<THeaders>
        where THeaders : HttpHeaders
    {
        ArgumentNullException.ThrowIfNull(name);
        return builder.ConfigureHeaders<TBuilder, THeaders>(headers => headers.Add(name, value));
    }

    [DebuggerStepThrough]
    public static T SetHeader<T>(this T builder, string name)
        where T : class, IHttpHeadersBuilder<HttpHeaders>
    {
        return builder.SetHeader<T, HttpHeaders>(name);
    }

    [DebuggerStepThrough]
    public static TBuilder SetHeader<TBuilder, THeaders>(this TBuilder builder, string name)
        where TBuilder : class, IHttpHeadersBuilder<THeaders>
        where THeaders : HttpHeaders
    {
        return builder.SetHeader<TBuilder, THeaders>(name, value: null);
    }

    [DebuggerStepThrough]
    public static T SetHeader<T>(this T builder, string name, string? value)
        where T : class, IHttpHeadersBuilder<HttpHeaders>
    {
        return builder.SetHeader<T, HttpHeaders>(name, value);
    }

    [DebuggerStepThrough]
    public static TBuilder SetHeader<TBuilder, THeaders>(this TBuilder builder, string name, string? value)
        where TBuilder : class, IHttpHeadersBuilder<THeaders>
        where THeaders : HttpHeaders
    {
        ArgumentNullException.ThrowIfNull(name);
        return builder
            .RemoveHeader<TBuilder, THeaders>(name)
            .AddHeader<TBuilder, THeaders>(name, value);
    }

    public static T SetReferer<T>(this T builder, string referer)
        where T : class, IHttpHeadersBuilder<HttpHeaders>
    {
        return builder.SetHeader("Referer", referer);
    }

    [DebuggerStepThrough]
    public static T RemoveHeader<T>(this T builder, params string?[]? names)
        where T : class, IHttpHeadersBuilder<HttpHeaders>
    {
        return builder.RemoveHeader<T, HttpHeaders>(names);
    }

    [DebuggerStepThrough]
    public static TBuilder RemoveHeader<TBuilder, THeaders>(this TBuilder builder, params string?[]? names)
        where TBuilder : class, IHttpHeadersBuilder<THeaders>
        where THeaders : HttpHeaders
    {
        return builder.ConfigureHeaders<TBuilder, THeaders>(headers => headers.Remove(names));
    }

    [DebuggerStepThrough]
    public static T ClearHeaders<T>(this T builder)
        where T : class, IHttpHeadersBuilder<HttpHeaders>
    {
        return builder.ClearHeaders<T, HttpHeaders>();
    }

    [DebuggerStepThrough]
    public static TBuilder ClearHeaders<TBuilder, THeaders>(this TBuilder builder)
        where TBuilder : class, IHttpHeadersBuilder<THeaders>
        where THeaders : HttpHeaders
    {
        return builder.ConfigureHeaders<TBuilder, THeaders>(h => h.Clear());
    }

    [DebuggerStepThrough]
    public static T ConfigureHeaders<T>(this T builder, Action<HttpHeaders> configureHeaders)
        where T : class, IHttpHeadersBuilder<HttpHeaders>
    {
        return builder.ConfigureHeaders<T, HttpHeaders>(configureHeaders);
    }

    [DebuggerStepThrough]
    public static TBuilder ConfigureHeaders<TBuilder, THeaders>(this TBuilder builder, Action<THeaders> configureHeaders)
        where TBuilder : class, IHttpHeadersBuilder<THeaders>
        where THeaders : HttpHeaders
    {
        ArgumentNullException.ThrowIfNull(configureHeaders);
        return builder.Configure(builder => configureHeaders(builder.Headers));
    }
}