//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Markup;
using System.Globalization;

namespace kyxsan.UI.Xaml.Markup;

[MarkupExtensionReturnType(ReturnType = typeof(string))]
internal sealed partial class ResourceStringExtension : MarkupExtension
{
    public SHName Name { get; set; }

    public string? CultureName { get; set; }

    protected override object ProvideValue()
    {
        CultureInfo cultureInfo = CultureName is not null ? CultureInfo.GetCultureInfo(CultureName) : CultureInfo.CurrentCulture;
        return SH.ResourceManager.GetString(string.Intern(Name.ToString()), cultureInfo) ?? string.Empty;
    }
}