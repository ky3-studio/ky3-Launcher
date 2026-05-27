//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

namespace kyxsan.Web.Request.Builder;

[Serializable]
internal readonly struct NoContent : IEquatable<NoContent>
{
    public static bool operator ==(NoContent a, NoContent b)
    {
        return Equals(a, b);
    }

    public static bool operator !=(NoContent a, NoContent b)
    {
        return !(a == b);
    }

    public override bool Equals(object? obj)
    {
        return obj is NoContent;
    }

    public bool Equals(NoContent other)
    {
        return true;
    }

    public override int GetHashCode()
    {
        return 1;
    }
}