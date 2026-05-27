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
using System.Net.Http;
using System.Net.Http.Headers;

namespace kyxsan.Web.Request.Builder;

internal sealed class HttpRequestMessageBuilder :
    IBuilder,
    IHttpRequestMessageBuilder,
    IHttpHeadersBuilder<HttpHeaders>,
    IHttpHeadersBuilder<HttpRequestHeaders>,
    IHttpHeadersBuilder<HttpContentHeaders>,
    IHttpContentBuilder,
    IHttpContentHeadersBuilder,
    IHttpRequestOptionsBuilder,
    IHttpProtocolVersionBuilder,
    IRequestUriBuilder,
    IHttpMethodBuilder
{
    public HttpRequestMessageBuilder(IServiceProvider serviceProvider, HttpContentSerializer httpContentSerializer, HttpRequestMessage? httpRequestMessage = default)
    {
        ServiceProvider = serviceProvider;
        HttpContentSerializer = httpContentSerializer;
        HttpRequestMessage = httpRequestMessage ?? new();
    }

    public HttpRequestMessage HttpRequestMessage
    {
        get;
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            field = value;
        }
    }

    public IServiceProvider ServiceProvider { get; }

    public HttpContentSerializer HttpContentSerializer { get; }

    HttpContentHeaders IHttpHeadersBuilder<HttpContentHeaders>.Headers
    {
        get
        {
            ArgumentNullException.ThrowIfNull(Content);
            return Content.Headers;
        }
    }

    HttpHeaders IHttpHeadersBuilder<HttpHeaders>.Headers
    {
        get => Headers;
    }

    public HttpRequestHeaders Headers
    {
        get => HttpRequestMessage.Headers;
    }

    public HttpRequestOptions Options
    {
        get => HttpRequestMessage.Options;
    }

    public HttpContent? Content
    {
        get => HttpRequestMessage.Content;
        set => HttpRequestMessage.Content = value;
    }

    public Version Version
    {
        get => HttpRequestMessage.Version;
        set => HttpRequestMessage.Version = value;
    }

    public Uri? RequestUri
    {
        get => HttpRequestMessage.RequestUri;
        set => HttpRequestMessage.RequestUri = value;
    }

    public HttpMethod Method
    {
        get => HttpRequestMessage.Method;
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            HttpRequestMessage.Method = value;
        }
    }

    public override string ToString()
    {
        return HttpRequestMessage.ToString();
    }
}