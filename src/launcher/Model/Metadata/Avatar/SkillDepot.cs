//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Model.Intrinsic;
using System.Collections.Immutable;

namespace kyxsan.Model.Metadata.Avatar;

internal sealed class SkillDepot
{
    public required Arkhe Arkhe { get; init; }

    public required ImmutableArray<ProudSkill> Skills { get; init; }

    public required ProudSkill EnergySkill { get; init; }

    public required ImmutableArray<ProudSkill> Inherents { get; init; }

    public required ImmutableArray<Skill> Talents { get; init; }

    public ImmutableArray<ProudSkill> CompositeSkills { get => !field.IsDefault ? field : field = [.. Skills, EnergySkill, .. Inherents]; }

    // No Inherents && 跳过 替换冲刺的技能
    public ImmutableArray<ProudSkill> CompositeSkillsNoInherents { get => !field.IsDefault ? field : field = [.. Skills.Where(s => s.Proud.Parameters.Count > 1), EnergySkill]; }
}