//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Core.ExceptionService;

namespace kyxsan.Core.Caching;

internal class InternalImageCacheException : Exception, IInternalException
{
    private InternalImageCacheException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    private InternalImageCacheException(string message)
        : base(message)
    {
    }

    public static InternalImageCacheException Throw(string message, Exception innerException)
    {
        throw new InternalImageCacheException(message, innerException);
    }

    public static InternalImageCacheException Throw(string message)
    {
        throw new InternalImageCacheException(message);
    }
}