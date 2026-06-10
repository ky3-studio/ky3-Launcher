//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using Microsoft.Windows.AppLifecycle;
using Microsoft.Windows.AppNotifications;
using Windows.ApplicationModel.Activation;

namespace kyxsan.Core.LifeCycle;

internal static class AppActivationArgumentsExtensions
{
    extension(AppActivationArguments activatedEventArgs)
    {
        public bool TryGetProtocolActivatedUri([NotNullWhen(true)] out Uri? uri)
        {
            uri = null;
            if (activatedEventArgs.Data is not IProtocolActivatedEventArgs protocolArgs)
            {
                return false;
            }

            uri = protocolArgs.Uri;
            return true;
        }

        public bool TryGetLaunchActivatedArguments([NotNullWhen(true)] out string? arguments)
        {
            arguments = null;

            if (activatedEventArgs.Data is not ILaunchActivatedEventArgs launchArgs)
            {
                return false;
            }

            arguments = launchArgs.Arguments.Trim();
            return true;
        }

        public bool TryGetAppNotificationActivatedArguments(out string? argument, [NotNullWhen(true)] out IDictionary<string, string>? arguments, [NotNullWhen(true)] out IDictionary<string, string>? userInput)
        {
            argument = null;
            arguments = null;
            userInput = null;

            if (activatedEventArgs.Data is not AppNotificationActivatedEventArgs appNotificationArgs)
            {
                return false;
            }

            argument = appNotificationArgs.Argument;
            arguments = appNotificationArgs.Arguments;
            userInput = appNotificationArgs.UserInput;
            return true;
        }
    }
}