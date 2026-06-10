//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Service.Notification;

namespace kyxsan.Web.Response;

internal sealed class DefaultResponseValidator : ICommonResponseValidator<Response>
{
    private readonly IMessenger messenger;

    public DefaultResponseValidator(IMessenger messenger)
    {
        this.messenger = messenger;
    }

    public bool TryValidate(Response response)
    {
        if (response.ReturnCode is 0)
        {
            return true;
        }

        if (response.ReturnCode is not Response.InternalFailure)
        {
            messenger.Send(InfoBarMessage.Error(response.ToString()));
        }

        return false;
    }

    public bool TryValidateWithoutUINotification(Response response)
    {
        return response.ReturnCode is 0;
    }
}