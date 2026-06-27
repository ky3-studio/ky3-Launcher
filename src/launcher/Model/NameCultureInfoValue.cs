//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using System.Globalization;

namespace Launcher.Model;

internal sealed class NameCultureInfoValue : NameValue<CultureInfo>
{
    public NameCultureInfoValue(string name, CultureInfo value, LocalizationSource localizationSource)
        : base(name, value)
    {
        IsMaintainedByLauncher = localizationSource.HasFlag(LocalizationSource.Launcher);
        IsMaintainedByCrowdin = localizationSource.HasFlag(LocalizationSource.Crowdin);
        IsMaintainedByGemini = localizationSource.HasFlag(LocalizationSource.Gemini);
    }

    public bool IsMaintainedByLauncher { get; }

    public bool IsMaintainedByCrowdin { get; }

    public bool IsMaintainedByGemini { get; }
}