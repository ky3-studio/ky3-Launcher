//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace kyxsan.Model.Entity;

[Table("avatar_strategies")]
internal sealed class AvatarStrategy
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public uint AvatarId { get; set; }

    public int ChineseStrategyId { get; set; }

    public int OverseaStrategyId { get; set; }

    [NotMapped]
    public Uri ChineseStrategyUrl { get => $"https://www.miyoushe.com/ys/article/{ChineseStrategyId}".ToUri(); }

    [NotMapped]
    public Uri OverseaStrategyUrl { get => $"https://www.hoyolab.com/article/{OverseaStrategyId}".ToUri(); }
}
