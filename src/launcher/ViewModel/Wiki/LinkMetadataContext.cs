//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Model.Metadata;
using kyxsan.Model.Metadata.Avatar;
using kyxsan.Model.Primitive;
using kyxsan.UI.Xaml.Control.TextBlock.Syntax.MiHoYo;
using System.Collections.Immutable;

namespace kyxsan.ViewModel.Wiki;

internal sealed class LinkMetadataContext
{
    public ImmutableDictionary<HyperLinkNameId, HyperLinkName> IdNameMap { get; init; } = default!;

    public ImmutableArray<ProudSkill> Skills { get; init; }

    public ImmutableArray<ProudSkill> Inherents { get; init; }

    public bool TryGetNameAndDescription(MiHoYoSyntaxLinkKind kind, uint id, out string name, out string description)
    {
        name = default!;
        description = default!;

        switch (kind)
        {
            case MiHoYoSyntaxLinkKind.Name:
                if (!IdNameMap.TryGetValue(id, out HyperLinkName? hyperLinkName))
                {
                    return false;
                }

                name = hyperLinkName.Name;
                description = hyperLinkName.Description;
                break;
            case MiHoYoSyntaxLinkKind.Inherent:
                ProudSkill? inherent = Inherents.SingleOrDefault(s => s.Id == id);
                if (inherent is null)
                {
                    return false;
                }

                name = inherent.Name;
                description = inherent.Description;
                break;
            case MiHoYoSyntaxLinkKind.Skill:
                ProudSkill? skill = Skills.SingleOrDefault(s => s.Id == id);
                if (skill is null)
                {
                    return false;
                }

                name = skill.Name;
                description = skill.Description;
                break;
            default:
                return false;
        }

        return true;
    }

    public bool TryGetParameter(MiHoYoSyntaxParameterKind kind, ReadOnlySpan<char> idSpan, out string value)
    {
        value = default!;

        if (!idSpan.TrySplitIntoTwo('|', out ReadOnlySpan<char> idSpan2, out ReadOnlySpan<char> nextSpan))
        {
            return false;
        }

        if (!uint.TryParse(idSpan2[1..], out uint id))
        {
            return false;
        }

        if (!nextSpan.TrySplitIntoTwo('S', out ReadOnlySpan<char> oneBasedIndexSpan, out ReadOnlySpan<char> factorSpan))
        {
            return false;
        }

        if (!int.TryParse(oneBasedIndexSpan, out int oneBasedIndex))
        {
            return false;
        }

        if (!int.TryParse(factorSpan, out int factor))
        {
            return false;
        }

        switch (kind)
        {
            case MiHoYoSyntaxParameterKind.ProudSkill:
                foreach (ProudSkill skill in Skills)
                {
                    foreach ((ProudSkillId skillId, ImmutableArray<float> parameters) in skill.Proud.Parameters.IdParameters)
                    {
                        if (skillId == id)
                        {
                            value = (parameters[oneBasedIndex - 1] * factor).ToString();
                            return true;
                        }
                    }
                }

                break;
            default:
                return false;
        }

        return false;
    }
}