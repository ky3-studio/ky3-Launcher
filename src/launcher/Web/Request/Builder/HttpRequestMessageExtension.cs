//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace kyxsan.Web.Request.Builder;

internal static class HttpRequestMessageExtension
{
    private const int MessageNotYetSent = 0;

    extension(HttpRequestMessage httpRequestMessage)
    {
        public void Resurrect()
        {
            // Reset send status
            Interlocked.Exchange(ref GetPrivateSendStatus(httpRequestMessage), MessageNotYetSent);

            if (httpRequestMessage.Content is { } content)
            {
                // Reflection workaround: https://github.com/dotnet/runtime/issues/119664
                typeof(HttpContent).GetField("_bufferedContent", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(content, null);
                Volatile.Write(ref GetPrivateDisposed(content), false);
            }
        }
    }

    // private int _sendStatus
    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_sendStatus")]
    private static extern ref int GetPrivateSendStatus(HttpRequestMessage message);

    // private bool _disposed
    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_disposed")]
    private static extern ref bool GetPrivateDisposed(HttpContent content);
}