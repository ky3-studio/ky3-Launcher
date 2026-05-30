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
using System.Net.Http;

namespace kyxsan.Web.Request.Builder;

internal static class HttpMethodBuilderExtension
{
    extension<T>(T builder)
        where T : class, IHttpMethodBuilder
    {
        [DebuggerStepThrough]
        public T Get()
        {
            return builder.SetMethod(HttpMethod.Get);
        }

        [DebuggerStepThrough]
        public T Post()
        {
            return builder.SetMethod(HttpMethod.Post);
        }

        [DebuggerStepThrough]
        public T Put()
        {
            return builder.SetMethod(HttpMethod.Put);
        }

        [DebuggerStepThrough]
        public T Delete()
        {
            return builder.SetMethod(HttpMethod.Delete);
        }

        [DebuggerStepThrough]
        public T Options()
        {
            return builder.SetMethod(HttpMethod.Options);
        }

        [DebuggerStepThrough]
        public T Trace()
        {
            return builder.SetMethod(HttpMethod.Trace);
        }

        [DebuggerStepThrough]
        public T Head()
        {
            return builder.SetMethod(HttpMethod.Head);
        }

        [DebuggerStepThrough]
        public T Patch()
        {
            return builder.SetMethod(HttpMethod.Patch);
        }

        [DebuggerStepThrough]
        public T SetMethod(string method)
        {
            ArgumentNullException.ThrowIfNull(method);
            return builder.SetMethod(new HttpMethod(method));
        }

        [DebuggerStepThrough]
        public T SetMethod(HttpMethod method)
        {
            ArgumentNullException.ThrowIfNull(method);
            return builder.Configure(builder => builder.Method = method);
        }
    }
}