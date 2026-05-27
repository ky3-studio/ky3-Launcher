//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Web.Request.Builder.Abstraction;
using System.Text;

namespace kyxsan.Web.Request.Builder;

internal static class JsonBuilderExtension
{
    extension<TBuilder>(TBuilder builder)
        where TBuilder : class, IHttpContentBuilder
    {
        public TBuilder SetJsonContent<TContent>(TContent content, Encoding? encoding = null, JsonHttpContentSerializer? serializer = null)
        {
            return builder.SetContent(serializer ?? builder.HttpContentSerializer, content, encoding);
        }

        public TBuilder SetJsonContent(object? content, Type contentType, Encoding? encoding = null, JsonHttpContentSerializer? serializer = null)
        {
            return builder.SetContent(serializer ?? builder.HttpContentSerializer, content, contentType, encoding);
        }
    }

    extension<TBuilder>(TBuilder builder)
        where TBuilder : class, IHttpMethodBuilder, IHttpContentBuilder
    {
        public TBuilder PostJson<TContent>(TContent content, Encoding? encoding = null, JsonHttpContentSerializer? serializer = null)
        {
            return builder.Post().SetJsonContent(content, encoding, serializer);
        }

        public TBuilder PostJson(object? content, Type contentType, Encoding? encoding = null, JsonHttpContentSerializer? serializer = null)
        {
            return builder.Post().SetJsonContent(content, contentType, encoding, serializer);
        }

        public TBuilder PutJson<TContent>(TContent content, Encoding? encoding = null, JsonHttpContentSerializer? serializer = null)
        {
            return builder.Put().SetJsonContent(content, encoding, serializer);
        }

        public TBuilder PutJson(object? content, Type contentType, Encoding? encoding = null, JsonHttpContentSerializer? serializer = null)
        {
            return builder.Put().SetJsonContent(content, contentType, encoding, serializer);
        }

        public TBuilder PatchJson<TContent>(TContent content, Encoding? encoding = null, JsonHttpContentSerializer? serializer = null)
        {
            return builder.Patch().SetJsonContent(content, encoding, serializer);
        }

        public TBuilder PatchJson(object? content, Type contentType, Encoding? encoding = null, JsonHttpContentSerializer? serializer = null)
        {
            return builder.Patch().SetJsonContent(content, contentType, encoding, serializer);
        }
    }
}