//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Core.DependencyInjection.Annotation.HttpClient;
using kyxsan.Core.IO;
using kyxsan.Web.Endpoint.kyxsan;
using kyxsan.Web.Request.Builder;
using kyxsan.Web.Request.Builder.Abstraction;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Mime;

namespace kyxsan.Core.Caching;

[Service(ServiceLifetime.Singleton, typeof(IImageCacheDownloadOperation))]
[HttpClient(HttpClientConfiguration.Default)]
[PrimaryHttpMessageHandler(MaxConnectionsPerServer = 8)]
internal sealed partial class ImageCacheDownloadOperation : IImageCacheDownloadOperation
{
    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    private readonly IHttpClientFactory httpClientFactory;

    [GeneratedConstructor]
    public partial ImageCacheDownloadOperation(IServiceProvider serviceProvider);

    public async ValueTask DownloadFileAsync(Uri uri, ValueFile baseFile)
    {
        using (HttpClient httpClient = httpClientFactory.CreateClient(nameof(ImageCacheDownloadOperation)))
        {
            try
            {
                await DownloadFileUsingHttpClientAsync(httpClient, uri, baseFile).ConfigureAwait(false);
            }
            catch (HttpRequestException)
            {
                foreach (Uri fallbackUri in StaticResourcesEndpoints.GetFallbackUris(uri))
                {
                    try
                    {
                        await DownloadFileUsingHttpClientAsync(httpClient, fallbackUri, baseFile).ConfigureAwait(false);
                        break;
                    }
                    catch (HttpRequestException)
                    {
                    }
                }
            }
        }

        if (!File.Exists(baseFile))
        {
            throw InternalImageCacheException.Throw($"'{uri.OriginalString}': File not exists after download attempt.");
        }
    }

    private async ValueTask DownloadFileUsingHttpClientAsync(HttpClient httpClient, Uri uri, ValueFile baseFile)
    {
        HttpRequestMessageBuilder requestMessageBuilder = httpRequestMessageBuilderFactory
            .Create()
            .SetRequestUri(uri)
            .Get();

        using (HttpRequestMessage requestMessage = requestMessageBuilder.HttpRequestMessage)
        {
            using (HttpResponseMessage responseMessage = await httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false))
            {
                if (!responseMessage.IsSuccessStatusCode)
                {
                    throw responseMessage.StatusCode switch
                    {
                        HttpStatusCode.NotFound => InternalImageCacheException.Throw($"Unable to download file from '{uri.OriginalString}': 404 Not Found"),
                        _ => InternalImageCacheException.Throw($"Unexpected HTTP status code {responseMessage.StatusCode}"),
                    };
                }

                if (responseMessage.Content.Headers.ContentType?.MediaType is MediaTypeNames.Application.Json)
                {
                    InternalImageCacheException.Throw($"Unexpected content type: {MediaTypeNames.Application.Json}");
                }

                using (Stream httpStream = await responseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false))
                {
                    string? directoryName = Path.GetDirectoryName(baseFile);
                    ArgumentException.ThrowIfNullOrEmpty(directoryName);

                    try
                    {
                        Directory.CreateDirectory(directoryName);
                    }
                    catch (DirectoryNotFoundException dnfEx)
                    {
                        throw InternalImageCacheException.Throw($"Unable to create folder at '{directoryName}'", dnfEx);
                    }

                    FileStream fileStream;
                    try
                    {
                        fileStream = File.Create(baseFile);
                    }
                    catch (IOException ex)
                    {
                        // The process cannot access the file '?' because it is being used by another process.
                        throw InternalImageCacheException.Throw($"Unable to create file at '{baseFile}'", ex);
                    }

                    try
                    {
                        using (fileStream)
                        {
                            await httpStream.CopyToAsync(fileStream).ConfigureAwait(false);
                        }
                    }
                    catch (IOException ex)
                    {
                        // Received an unexpected EOF or 0 bytes from the transport stream.
                        // Unable to read data from the transport connection: 远程主机强迫关闭了一个现有的连接。. SocketException: ConnectionReset
                        // Unable to read data from the transport connection: 你的主机中的软件中止了一个已建立的连接。. SocketException: ConnectionAborted
                        // HttpIOException: The response ended prematurely. (ResponseEnded)
                        // 磁盘空间不足。 : '?'.
                        throw InternalImageCacheException.Throw("Unable to copy stream content to file", ex);
                    }
                }
            }
        }
    }
}