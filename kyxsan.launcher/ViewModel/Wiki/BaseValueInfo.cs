//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using kyxsan.Model;
using kyxsan.Model.Metadata.Converter;
using kyxsan.Model.Primitive;
using System.Collections.Immutable;

namespace kyxsan.ViewModel.Wiki;

internal sealed partial class BaseValueInfo : ObservableObject
{
    private readonly ImmutableArray<PropertyCurveValue> propValues;
    private readonly BaseValueInfoMetadataContext metadataContext;

    public BaseValueInfo(uint maxLevel, ImmutableArray<PropertyCurveValue> propValues, BaseValueInfoMetadataContext metadataContext)
    {
        this.propValues = propValues;
        this.metadataContext = metadataContext;

        MaxLevel = maxLevel;
        CurrentLevel = maxLevel;
    }

    public uint MaxLevel { get; }

    [ObservableProperty]
    public partial ImmutableArray<NameValue<string>> Values { get; set; } = [];

    public uint CurrentLevel
    {
        get;
        set
        {
            if (SetProperty(ref field, value))
            {
                OnPropertyChanged(nameof(CurrentLevelFormatted));
                UpdateValues(value, Promoted);
            }
        }
    }

    public string CurrentLevelFormatted { get => LevelFormat.Format(CurrentLevel); }

    public bool Promoted
    {
        get;
        set
        {
            if (SetProperty(ref field, value))
            {
                UpdateValues(CurrentLevel, value);
            }
        }
    }

    private void UpdateValues(Level level, bool promoted)
    {
        Values = BaseValueInfoConverter.ToNameValues(propValues, level, MaxLevel, promoted, metadataContext);
    }
}