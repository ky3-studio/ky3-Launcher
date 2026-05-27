//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Markup;

namespace kyxsan.UI.Xaml.Markup;

internal static class XamlServiceProviderExtension
{
    extension(IXamlServiceProvider provider)
    {
        public IProvideValueTarget GetProvideValueTarget()
        {
            return (IProvideValueTarget)provider.GetService(typeof(IProvideValueTarget));
        }

        public IRootObjectProvider GetRootObjectProvider()
        {
            return (IRootObjectProvider)provider.GetService(typeof(IRootObjectProvider));
        }

        public IUriContext GetUriContext()
        {
            return (IUriContext)provider.GetService(typeof(IUriContext));
        }

        public IXamlTypeResolver GetXamlTypeResolver()
        {
            return (IXamlTypeResolver)provider.GetService(typeof(IXamlTypeResolver));
        }
    }
}