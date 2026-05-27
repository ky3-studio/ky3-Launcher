//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using JetBrains.Annotations;
using kyxsan.Core.IO.Http;
using kyxsan.Core.IO.Http.Proxy;
using kyxsan.Service.Game.Package.Advanced;
using kyxsan.Web.Hoyolab;
using kyxsan.Win32;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Text;

namespace kyxsan.Core.DependencyInjection;

// ReSharper disable UnusedMember.Local
internal static partial class ServiceCollectionExtension
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static partial IServiceCollection AddHttpClients(this IServiceCollection services);

    extension(IServiceCollection services)
    {
        public IServiceCollection AddConfiguredHttpClients()
        {
            services
                .ConfigureHttpClientDefaults(clientBuilder =>
                {
                    clientBuilder
                        .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler())
                        .ConfigurePrimaryHttpMessageHandler((handler, provider) =>
                        {
                            SocketsHttpHandler typedHandler = Unsafe.As<SocketsHttpHandler>(handler);
                            typedHandler.UseProxy = true;
                            typedHandler.Proxy = HttpProxyUsingSystemProxy.Instance;
                            typedHandler.SslOptions = new System.Net.Security.SslClientAuthenticationOptions
                            {
                                RemoteCertificateValidationCallback = (_, _, _, _) => true
                            };
                        })
                        .AddHttpMessageHandler<RetryHttpHandler>();
                })
                .AddHttpClients();

            services
                .AddHttpClient(GamePackageService.HttpClientName)
                .ConfigureHttpClient(httpClient =>
                {
                    httpClient.DefaultRequestVersion = HttpVersion.Version20;
                })
                .ConfigurePrimaryHttpMessageHandler((handler, provider) =>
                {
                    SocketsHttpHandler typedHandler = Unsafe.As<SocketsHttpHandler>(handler);
                    typedHandler.ConnectTimeout = TimeSpan.FromSeconds(30);
                    typedHandler.MaxConnectionsPerServer = 32;
                });

            return services;
        }
    }

    [UsedImplicitly]
    private static void DefaultConfiguration(IServiceProvider serviceProvider, HttpClient client)
    {
        client.Timeout = Timeout.InfiniteTimeSpan;
        client.DefaultRequestHeaders.UserAgent.ParseAdd(kyxsanRuntime.UserAgent);
        client.DefaultRequestHeaders.Add("x-kyxsan-device-id", kyxsanRuntime.DeviceId);
        client.DefaultRequestHeaders.Add("x-kyxsan-device-os", $"Windows {kyxsanNative.Instance.GetCurrentWindowsVersion()}");
        client.DefaultRequestHeaders.Add("x-kyxsan-device-name", EncodeNonAsciiChars(Environment.MachineName));
    }

    [UsedImplicitly]
    private static void XRpcConfiguration(HttpClient client)
    {
        client.Timeout = Timeout.InfiniteTimeSpan;
        client.DefaultRequestHeaders.UserAgent.ParseAdd(HoyolabOptions.UserAgent);
        client.DefaultRequestHeaders.Accept.ParseAdd(MediaTypeNames.Application.Json);
        client.DefaultRequestHeaders.Add("x-rpc-app_version", SaltConstants.CNVersion);
        client.DefaultRequestHeaders.Add("x-rpc-client_type", "5");
        client.DefaultRequestHeaders.Add("x-rpc-device_id", HoyolabOptions.DeviceId36);
    }

    [UsedImplicitly]
    private static void XRpc2Configuration(HttpClient client)
    {
        client.Timeout = Timeout.InfiniteTimeSpan;
        client.DefaultRequestHeaders.UserAgent.ParseAdd(HoyolabOptions.UserAgent);
        client.DefaultRequestHeaders.Accept.ParseAdd(MediaTypeNames.Application.Json);
        client.DefaultRequestHeaders.Add("x-rpc-aigis", string.Empty);
        client.DefaultRequestHeaders.Add("x-rpc-app_id", "bll8iq97cem8");
        client.DefaultRequestHeaders.Add("x-rpc-app_version", SaltConstants.CNVersion);
        client.DefaultRequestHeaders.Add("x-rpc-client_type", "2");
        client.DefaultRequestHeaders.Add("x-rpc-device_id", HoyolabOptions.DeviceId36);
        client.DefaultRequestHeaders.Add("x-rpc-device_name", string.Empty);
        client.DefaultRequestHeaders.Add("x-rpc-game_biz", "bbs_cn");
        client.DefaultRequestHeaders.Add("x-rpc-sdk_version", "2.16.0");
    }

    [UsedImplicitly]
    private static void XRpc3Configuration(HttpClient client)
    {
        client.Timeout = Timeout.InfiniteTimeSpan;
        client.DefaultRequestHeaders.UserAgent.ParseAdd(HoyolabOptions.UserAgentOversea);
        client.DefaultRequestHeaders.Accept.ParseAdd(MediaTypeNames.Application.Json);
        client.DefaultRequestHeaders.Add("x-rpc-app_version", SaltConstants.OSVersion);
        client.DefaultRequestHeaders.Add("x-rpc-client_type", "5");
        client.DefaultRequestHeaders.Add("x-rpc-language", "zh-cn");
        client.DefaultRequestHeaders.Add("x-rpc-device_id", HoyolabOptions.DeviceId36);
    }

    [UsedImplicitly]
    [SuppressMessage("", "IDE0051")]
    private static void XRpc4Configuration(HttpClient client)
    {
        client.Timeout = Timeout.InfiniteTimeSpan;
        client.DefaultRequestHeaders.UserAgent.ParseAdd(HoyolabOptions.UserAgentOversea);
        client.DefaultRequestHeaders.Accept.ParseAdd(MediaTypeNames.Application.Json);
        client.DefaultRequestHeaders.Add("x-rpc-app_version", "1.5.0");
        client.DefaultRequestHeaders.Add("x-rpc-client_type", "4");
    }

    [UsedImplicitly]
    private static void XRpc5Configuration(HttpClient client)
    {
        client.Timeout = Timeout.InfiniteTimeSpan;
        client.DefaultRequestHeaders.UserAgent.ParseAdd(HoyolabOptions.HoyoPlayUserAgent);
        client.DefaultRequestHeaders.Accept.ParseAdd(MediaTypeNames.Application.Json);
        client.DefaultRequestHeaders.Add("x-rpc-app_id", "ddxf5dufpuyo");
        client.DefaultRequestHeaders.Add("x-rpc-client_type", "3");
    }

    [UsedImplicitly]
    private static void XRpc6Configuration(HttpClient client)
    {
        client.Timeout = Timeout.InfiniteTimeSpan;
        client.DefaultRequestHeaders.UserAgent.ParseAdd(HoyolabOptions.HoyoPlayUserAgent);
        client.DefaultRequestHeaders.Accept.ParseAdd(MediaTypeNames.Application.Json);
        client.DefaultRequestHeaders.Add("x-rpc-app_id", "ddxf6vlr1reo");
        client.DefaultRequestHeaders.Add("x-rpc-client_type", "3");
        client.DefaultRequestHeaders.Add("x-rpc-device_id", HoyolabOptions.DeviceId53);
    }

    private static string EncodeNonAsciiChars(string value)
    {
        StringBuilder sb = new();
        foreach (char c in value)
        {
            if (c > 127)
            {
                sb.Append("\\u").Append(((int)c).ToString("x4", CultureInfo.InvariantCulture));
            }
            else
            {
                sb.Append(c);
            }
        }

        return sb.ToString();
    }
}