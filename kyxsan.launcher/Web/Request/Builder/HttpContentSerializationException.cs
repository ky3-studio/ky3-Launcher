//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Core.ExceptionService;
using System.Net.Http;

namespace kyxsan.Web.Request.Builder;

[Serializable]
internal sealed class HttpContentSerializationException : Exception
{
    public HttpContentSerializationException(string? message, Exception? innerException)
        : base(message ?? GetDefaultMessage(), innerException)
    {
    }

    private HttpContentSerializationException(Exception? innerException)
        : base(GetDefaultMessage(), innerException)
    {
    }

    public static async ValueTask<HttpContentSerializationException> CreateAsync(HttpContent? content, Exception? innerException)
    {
        if (content is null)
        {
            return new(innerException);
        }

        HttpContentSerializationException exception = new(GetDefaultMessage(), innerException);

        // Read the content into array, in case the response disposed.
        ExceptionAttachment.SetAttachment(exception, "content.txt", await content.ReadAsByteArrayAsync().ConfigureAwait(false));
        return exception;
    }

    private static string GetDefaultMessage()
    {
        return """
            The (de-)serialization failed because of an arbitrary error. This most likely happened, 
            because an inner serializer failed to (de-)serialize the given data. 
            See the inner exception for details (if available).
            """;
    }
}