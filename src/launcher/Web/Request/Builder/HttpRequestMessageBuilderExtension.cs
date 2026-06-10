//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Service.Notification;
using kyxsan.Web.Response;
using System.IO;
using System.Net.Http;
using System.Net.Sockets;
using System.Runtime.ExceptionServices;
using System.Text;

namespace kyxsan.Web.Request.Builder;

internal static class HttpRequestMessageBuilderExtension
{
    extension(HttpRequestMessageBuilder builder)
    {
        internal HttpRequestMessageBuilder Resurrect()
        {
            builder.HttpRequestMessage.Resurrect();
            return builder;
        }

        internal ValueTask<TypedHttpResponse<TResult>> SendAsync<TResult>(HttpClient httpClient, CancellationToken token)
            where TResult : class
        {
            return SendAsync<TResult>(builder, httpClient, HttpCompletionOption.ResponseContentRead, true, token);
        }

        internal ValueTask<TypedHttpResponse<TResult>> SendAsync<TResult>(HttpClient httpClient, bool logException, CancellationToken token)
            where TResult : class
        {
            return SendAsync<TResult>(builder, httpClient, HttpCompletionOption.ResponseContentRead, logException, token);
        }

        internal ValueTask<TypedHttpResponse<TResult>> SendAsync<TResult>(HttpClient httpClient, HttpCompletionOption completionOption, CancellationToken token)
            where TResult : class
        {
            return SendAsync<TResult>(builder, httpClient, completionOption, true, token);
        }

        internal async ValueTask<TypedHttpResponse<TResult>> SendAsync<TResult>(HttpClient httpClient, HttpCompletionOption completionOption, bool logException, CancellationToken token)
            where TResult : class
        {
            Uri? requestUri = builder.HttpRequestMessage.RequestUri;
            if (requestUri is null || !requestUri.IsAbsoluteUri)
            {
                return new(null, null);
            }

            HttpContext context = new()
            {
                HttpClient = httpClient,
                CompletionOption = completionOption,
                RequestAborted = token,
            };

            using (context)
            {
                await SendAsync(builder, context).ConfigureAwait(false);

                StringBuilder messageBuilder = new();
                bool showInfo = true;

                try
                {
                    context.Exception?.Throw();
                    ArgumentNullException.ThrowIfNull(context.Response);

                    showInfo = false;
                    TResult? body = await builder.HttpContentSerializer.DeserializeAsync<TResult>(context.Response.Content, token).ConfigureAwait(false);
                    return new(context.Response.Headers, body);
                }
                catch (OperationCanceledException)
                {
                    showInfo = false;

                    // Populate to caller
                    throw;
                }
                catch (Exception ex)
                {
                    if (logException)
                    {
                        HttpRequestExceptionHandling.TryHandle(messageBuilder, builder, ex);
                    }

                    return new(context.Response?.Headers, default);
                }
                finally
                {
                    if (showInfo && logException)
                    {
                        builder.ServiceProvider.GetRequiredService<IMessenger>().Send(InfoBarMessage.Error(messageBuilder.ToString()));
                    }
                }
            }
        }

        internal async ValueTask SendAsync(HttpContext context)
        {
            Uri? requestUri = builder.HttpRequestMessage.RequestUri;
            if (requestUri is null || !requestUri.IsAbsoluteUri)
            {
                return;
            }

            try
            {
                context.Request = builder.HttpRequestMessage;
                context.Response = await context.HttpClient.SendAsync(context.Request, context.CompletionOption, context.RequestAborted).ConfigureAwait(false);
                context.Response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                context.Exception = ExceptionDispatchInfo.Capture(ex);
            }
        }

        internal void Send(HttpClient httpClient)
        {
            Uri? requestUri = builder.HttpRequestMessage.RequestUri;
            if (requestUri is null || !requestUri.IsAbsoluteUri)
            {
                return;
            }

            try
            {
                using (httpClient.Send(builder.HttpRequestMessage))
                {
                }
            }
            catch (HttpRequestException)
            {
            }
            catch (IOException)
            {
            }
            catch (JsonException)
            {
            }
            catch (HttpContentSerializationException)
            {
            }
            catch (SocketException)
            {
            }
        }
    }
}