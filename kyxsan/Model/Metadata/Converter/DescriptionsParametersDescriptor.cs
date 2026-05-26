//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Core.ExceptionService;
using kyxsan.Model.Metadata.Avatar;
using kyxsan.Model.Primitive;
using kyxsan.UI.Xaml.Data.Converter;
using System.Collections.Immutable;
using System.Globalization;
using System.Text.RegularExpressions;

namespace kyxsan.Model.Metadata.Converter;

internal sealed partial class DescriptionsParametersDescriptor : ValueConverter<DescriptionsParameters, IList<LevelParameters<string, ParameterDescription>>>
{
    [GeneratedRegex("{param([1-9][0-9]*?):(.+?)}")]
    private static partial Regex ParamRegex { get; }

    public static LevelParameters<string, ParameterDescription> Convert(DescriptionsParameters from, uint level)
    {
        return new(LevelFormat.Format(level), GetParameterDescription(from.Descriptions, from.Parameters[(SkillLevel)level]));
    }

    public override IList<LevelParameters<string, ParameterDescription>> Convert(DescriptionsParameters from)
    {
        return from.Parameters.Convert(from.Descriptions, GetParameterDescription);
    }

    private static ImmutableArray<ParameterDescription> GetParameterDescription(ImmutableArray<string> descriptions, ImmutableArray<float> paramArray)
    {
        ReadOnlySpan<string> span = descriptions.AsSpan();
        ImmutableArray<ParameterDescription>.Builder results = ImmutableArray.CreateBuilder<ParameterDescription>(span.Length);

        foreach (ref readonly string desc in span)
        {
            if (desc.AsSpan().TrySplitIntoTwo('|', out ReadOnlySpan<char> description, out ReadOnlySpan<char> format))
            {
                if (description[0] is not '#')
                {
                    // Fast path
                    string resultFormatted = ParamRegex.Replace(format.ToString(), match => ReplaceParamInMatch(match, paramArray));
                    results.Add(new(resultFormatted, description.ToString()));
                }
                else
                {
                    string descriptionString = SpecialNameHandling.Handle(description.ToString());
                    string formatString = SpecialNameHandling.Handle(format.ToString());

                    string resultFormatted = ParamRegex.Replace(formatString, match => ReplaceParamInMatch(match, paramArray));
                    results.Add(new(resultFormatted, descriptionString));
                }
            }
            else
            {
                kyxsanException.InvalidOperation($"ParameterFormat failed, value: `{desc}`");
            }
        }

        return results.ToImmutable();
    }

    private static string ReplaceParamInMatch(Match match, ImmutableArray<float> paramArray)
    {
        if (match.Success)
        {
            int index = int.Parse(match.Groups[1].Value, CultureInfo.CurrentCulture) - 1;
            return ParameterFormat.FormatInvariant($"{{0:{match.Groups[2].Value}}}", paramArray[index]);
        }

        return string.Empty;
    }
}