//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Launcher.Core.Database.Abstraction;
using Launcher.Model.Entity.Abstraction;
using Launcher.Model.Entity.Primitive;
using Launcher.UI.Xaml.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Launcher.Model.Entity;

[Table("game_accounts")]
internal sealed partial class GameAccount : ObservableObject,
    IAppDbEntity,
    IReorderable,
    IPropertyValuesProvider
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid InnerId { get; set; }

    public SchemeType Type { get; set; }

    public string Name { get; set; } = default!;

    /// <summary>
    /// [MIHOYOSDK_ADL_PROD_CN_h3123967166]
    /// [MIHOYOSDK_ADL_PROD_OVERSEA_h1158948810]
    /// </summary>
    public string MihoyoSDK { get; set; } = default!;

    public string? Mid { get; set; }

    public string? MacAddress { get; set; }

    public int Index { get; set; }

    [NotMapped]
    public bool IsExpired { get; set; }

    public static GameAccount Create(SchemeType type, string name, string sdk, string? mid, string? mac)
    {
        return new()
        {
            Type = type,
            Name = name,
            MihoyoSDK = sdk,
            MacAddress = mac,
            Mid = mid,
        };
    }

    public void UpdateName(string name)
    {
        Name = name;
        OnPropertyChanged(nameof(Name));
    }
}
