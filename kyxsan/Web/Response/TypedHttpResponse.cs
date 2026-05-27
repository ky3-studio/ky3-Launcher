//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Core.Abstraction;
using System.Net.Http.Headers;

namespace kyxsan.Web.Response;

internal sealed class TypedHttpResponse<TResult> : IDeconstruct<HttpResponseHeaders?, TResult?>
    where TResult : class
{
    public TypedHttpResponse(HttpResponseHeaders? headers, TResult? result)
    {
        Headers = headers;
        Body = result;
    }

    public HttpResponseHeaders? Headers { get; }

    public TResult? Body { get; }

    public static implicit operator TResult?(TypedHttpResponse<TResult> response)
    {
        return response.Body;
    }

    public void Deconstruct(out HttpResponseHeaders? headers, out TResult? body)
    {
        headers = Headers;
        body = Body;
    }
}