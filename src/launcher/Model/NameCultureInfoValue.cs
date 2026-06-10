//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using System.Globalization;

namespace kyxsan.Model;

internal sealed class NameCultureInfoValue : NameValue<CultureInfo>
{
    public NameCultureInfoValue(string name, CultureInfo value, LocalizationSource localizationSource)
        : base(name, value)
    {
        IsMaintainedBykyxsan = localizationSource.HasFlag(LocalizationSource.kyxsan);
        IsMaintainedByCrowdin = localizationSource.HasFlag(LocalizationSource.Crowdin);
        IsMaintainedByGemini = localizationSource.HasFlag(LocalizationSource.Gemini);
    }

    public bool IsMaintainedBykyxsan { get; }

    public bool IsMaintainedByCrowdin { get; }

    public bool IsMaintainedByGemini { get; }
}