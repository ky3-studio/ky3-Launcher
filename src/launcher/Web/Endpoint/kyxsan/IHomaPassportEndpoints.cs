//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

namespace kyxsan.Web.Endpoint.kyxsan;

internal interface IHomaPassportEndpoints : IHomaRootAccess
{
    string PassportVerify()
    {
        return $"{Root}/Passport/v2/Verify";
    }

    string PassportRegister()
    {
        return $"{Root}/Passport/v2/Register";
    }

    string PassportCancel()
    {
        return $"{Root}/Passport/v2/Cancel";
    }

    string PassportResetUserName()
    {
        return $"{Root}/Passport/v2/ResetUsername";
    }

    string PassportResetPassword()
    {
        return $"{Root}/Passport/v2/ResetPassword";
    }

    string PassportLogin()
    {
        return $"{Root}/Passport/v2/Login";
    }

    string PassportUserInfo()
    {
        return $"{Root}/Passport/v2/UserInfo";
    }

    string PassportRefreshToken()
    {
        return $"{Root}/Passport/v2/RefreshToken";
    }

    string PassportRevokeToken()
    {
        return $"{Root}/Passport/v2/RevokeToken";
    }

    string PassportRevokeAllTokens()
    {
        return $"{Root}/Passport/v2/RevokeAllTokens";
    }
}