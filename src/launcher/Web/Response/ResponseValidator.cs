//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by Launcher.
// Licensed under the MIT license.

namespace Launcher.Web.Response;

internal static class ResponseValidator
{
    public static bool TryValidate(Response response, IMessenger messenger)
    {
        return new DefaultResponseValidator(messenger).TryValidate(response);
    }

    public static bool TryValidate(Response response, IServiceProvider serviceProvider)
    {
        try
        {
            return serviceProvider.GetRequiredService<ICommonResponseValidator<Response>>().TryValidate(response);
        }
        catch (ObjectDisposedException)
        {
            return false;
        }
    }

    public static bool TryValidate<TData>(Response<TData> response, IMessenger messenger, [NotNullWhen(true)] out TData? data)
    {
        return new TypedResponseValidator<TData>(messenger).TryValidate(response, out data);
    }

    public static bool TryValidate<TData>(Response<TData> response, IServiceProvider serviceProvider, [NotNullWhen(true)] out TData? data)
    {
        try
        {
            return serviceProvider.GetRequiredService<ITypedResponseValidator<TData>>().TryValidate(response, out data);
        }
        catch (ObjectDisposedException)
        {
            data = default;
            return false;
        }
    }

    public static bool TryValidateWithoutUINotification(Response response)
    {
        return new DefaultResponseValidator(default!).TryValidateWithoutUINotification(response);
    }

    public static bool TryValidateWithoutUINotification(Response response, IServiceProvider serviceProvider)
    {
        try
        {
            return serviceProvider.GetRequiredService<ICommonResponseValidator<Response>>().TryValidateWithoutUINotification(response);
        }
        catch (ObjectDisposedException)
        {
            return false;
        }
    }

    public static bool TryValidateWithoutUINotification<TData>(Response<TData> response, [NotNullWhen(true)] out TData? data)
    {
        return new TypedResponseValidator<TData>(default!).TryValidateWithoutUINotification(response, out data);
    }

    public static bool TryValidateWithoutUINotification<TData>(Response<TData> response, IServiceProvider serviceProvider, [NotNullWhen(true)] out TData? data)
    {
        try
        {
            return serviceProvider.GetRequiredService<ITypedResponseValidator<TData>>().TryValidateWithoutUINotification(response, out data);
        }
        catch (ObjectDisposedException)
        {
            data = default;
            return false;
        }
    }
}