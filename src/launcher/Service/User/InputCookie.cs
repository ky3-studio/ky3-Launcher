//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Launcher.Core.Abstraction;
using Launcher.Web.Hoyolab;

namespace Launcher.Service.User;

internal sealed class InputCookie : IDeconstruct<Cookie, bool, string?>
{
    private InputCookie(Cookie cookie, bool isOversea)
    {
        Cookie = cookie;
        IsOversea = isOversea;
        cookie.TryGetDeviceFp(out string? deviceFp);
        DeviceFp = deviceFp;
    }

    public Cookie Cookie { get; }

    public bool IsOversea { get; }

    public string? DeviceFp { get; }

    public static InputCookie CreateForDeviceFpInference(Cookie cookie, bool isOversea)
    {
        return new(cookie, isOversea);
    }

    public void Deconstruct(out Cookie cookie, out bool isOversea, out string? deviceFp)
    {
        cookie = Cookie;
        isOversea = IsOversea;
        deviceFp = DeviceFp;
    }
}
