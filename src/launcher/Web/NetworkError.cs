//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

namespace kyxsan.Web;

// ReSharper disable InconsistentNaming
internal enum NetworkError
{
    NULL,
    ERR_CONNECTION_ABORTED,
    ERR_CONNECTION_ACCESS_DENIED,
    ERR_CONNECTION_ADDRESS_ALREADY_IN_USE,
    ERR_CONNECTION_ADDRESS_NOT_AVAILABLE,
    ERR_CONNECTION_HOST_UNREACHABLE,
    ERR_CONNECTION_REFUSED,
    ERR_CONNECTION_NETWORK_UNREACHABLE,
    ERR_CONNECTION_NO_BUFFER_SPACE_AVAILABLE,
    ERR_CONNECTION_NO_DATA,
    ERR_CONNECTION_NOT_SOCKET,
    ERR_CONNECTION_TIMED_OUT,
    ERR_NAME_RESOLUTION_HOST_NOT_FOUND,
    ERR_PROXY_TUNNEL_ERROR,
    ERR_RESPONSE_ENDED,
    ERR_SECURE_CONNECTION_ABORTED,
    ERR_SECURE_CONNECTION_AUTHENTICATION_ERROR,
    ERR_SECURE_CONNECTION_ERROR,
    ERR_SECURE_CONNECTION_RESET,
    ERR_SECURE_CONNECTION_SEC_E_ILLEGAL_MESSAGE,
    ERR_SECURE_CONNECTION_SEC_E_INTERNAL_ERROR,
    ERR_UNKNOWN,
    ERR_UNKNOWN_ACCESS_DENIED,
    ERR_UNKNOWN_AUTHENTICATION_ERROR,
    ERR_UNKNOWN_CONNECTION_ABORTED,
    ERR_UNKNOWN_CONNECTION_RESET,
    ERR_UNKNOWN_SEC_E_DECRYPT_FAILURE,
    ERR_UNKNOWN_SEC_E_MESSAGE_ALTERED,
    ERR_UNKNOWN_TIMED_OUT,
}