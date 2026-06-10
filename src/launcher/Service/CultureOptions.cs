//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Core.Property;
using kyxsan.Core.Setting;
using kyxsan.Model;
using kyxsan.Service.Abstraction;
using System.Collections.Immutable;
using System.Globalization;

namespace kyxsan.Service;

[Service(ServiceLifetime.Singleton)]
internal sealed partial class CultureOptions : DbStoreOptions
{
    [GeneratedConstructor(CallBaseConstructor = true)]
    public partial CultureOptions(IServiceProvider serviceProvider);

    public static ImmutableArray<NameCultureInfoValue> Cultures { get; } = SupportedCultures.GetValues();

    [field: MaybeNull]
    public IObservableProperty<CultureInfo> CurrentCulture { get => field ??= CreatePropertyForClassUsingCustom(SettingKeys.PrimaryLanguage, CultureInfo.CurrentCulture, CultureInfo.GetCultureInfo, static v => v.Name); }

    public CultureInfo SystemCulture { get; set; } = default!;

    public string LocaleName { get => LocaleNames.GetLocaleName(CurrentCulture.Value); }

    [field: AllowNull]
    [field: MaybeNull]
    public string LanguageCode
    {
        get
        {
            if (field is null && !LocaleNames.TryGetLanguageCodeFromLocaleName(LocaleName, out field))
            {
                throw new KeyNotFoundException($"Invalid localeName: '{LocaleName}'");
            }

            return field;
        }
    }
}