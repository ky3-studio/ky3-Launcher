//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using System.Diagnostics;

namespace kyxsan.Core.Abstraction;

internal static class BuilderExtension
{
    extension<T>(T builder)
        where T : class, IBuilder
    {
        [DebuggerStepThrough]
        public T Configure(Action<T> configure)
        {
            ArgumentNullException.ThrowIfNull(builder);
            ArgumentNullException.ThrowIfNull(configure);

            configure(builder);
            return builder;
        }

        [DebuggerStepThrough]
        public unsafe T Configure(delegate*<T, void> configure)
        {
            ArgumentNullException.ThrowIfNull(builder);
            ArgumentNullException.ThrowIfNull(configure);

            configure(builder);
            return builder;
        }

        [DebuggerStepThrough]
        public T If(bool condition, Action<T> action)
        {
            ArgumentNullException.ThrowIfNull(builder);
            ArgumentNullException.ThrowIfNull(action);

            if (condition)
            {
                action(builder);
            }

            return builder;
        }

        [DebuggerStepThrough]
        public unsafe T If(bool condition, delegate*<T, void> action)
        {
            ArgumentNullException.ThrowIfNull(builder);
            ArgumentNullException.ThrowIfNull(action);

            if (condition)
            {
                action(builder);
            }

            return builder;
        }
    }
}