using Launcher.Model.Intrinsic;
using Launcher.UI.Xaml.Data;
using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace Launcher.Model.Metadata.Food;

internal sealed partial class Food : IPropertyValuesProvider
{
    private static readonly Regex ColorTagRegex = new(@"<color=([^>]*)>(.*?)</color>", RegexOptions.Compiled);

    public required uint Id { get; init; }

    public required string Name { get; init; }

    public required int Rank { get; init; }

    public required string Icon { get; init; }

    public required string EffectIcon { get; init; }

    public required string Description { get; init; }

    public string? TypeName { get; init; }

    public FoodRecipe? Recipe { get; init; }

    public FoodCooking? Cooking { get; init; }

    public string? Character { get; init; }

    public string? CharacterIcon { get; init; }

    public int CharacterRank { get; init; }

    public string? BaseDish { get; init; }

    public string? BaseDishIcon { get; init; }

    [JsonIgnore]
    public Uri? BaseDishIconUri => BaseDishIcon is not null
        ? BaseDishIcon.StartsWith("Skill_", StringComparison.Ordinal)
            ? Converter.SkillIconConverter.IconNameToUri(BaseDishIcon)
            : Converter.ItemIconConverter.IconNameToUri(BaseDishIcon)
        : null;

    [JsonIgnore]
    public bool IsSpecialDish => Character is not null;

    [JsonIgnore]
    public QualityType CharacterQuality => (QualityType)CharacterRank;

    [JsonIgnore]
    public Uri? CharacterIconUri => CharacterIcon is not null
        ? Converter.AvatarIconConverter.IconNameToUri(CharacterIcon)
        : null;

    [JsonIgnore]
    public string? SpecialDishLabel => Character is not null
        ? BaseDish is not null
            ? $"{Character} | {BaseDish}"
            : $"{Character}"
        : null;

    [JsonIgnore]
    public string BaseDishOrSkillLabel => BaseDish
        ?? (BaseDishIcon is not null && BaseDishIcon.StartsWith("Skill_", StringComparison.Ordinal)
            ? SH.ViewWikiFoodElementalSkill
            : SH.ViewWikiFoodCategorySpecial);

    [JsonIgnore]
    public QualityType Quality => (QualityType)Rank;

    [JsonIgnore]
    public int InventoryCount { get; set; }

    [JsonIgnore]
    public string? EffectDescription
    {
        get
        {
            string? raw = Recipe?.Effect?.Values.FirstOrDefault();
            if (raw is null)
            {
                return null;
            }

            return ColorTagRegex.Replace(raw, m =>
            {
                string hex = m.Groups[1].Value.TrimStart('#');
                string rgb = hex.Length >= 6 ? hex[..6] : hex;
                return $"<color style='color:#{rgb};'>{m.Groups[2].Value}</color>";
            });
        }
    }
}

internal sealed class FoodRecipe
{
    public string? EffectIcon { get; init; }

    public Dictionary<string, string>? Effect { get; init; }

    public ImmutableArray<FoodRecipeInput> Input { get; init; } = [];
}

internal sealed class FoodRecipeInput
{
    public string? Name { get; init; }

    public string? Icon { get; init; }

    public int Count { get; init; }

    [JsonIgnore]
    public string CountText => $"x{Count}";
}

internal sealed class FoodCooking
{
    public double Center { get; init; }

    public double Width { get; init; }

    [JsonIgnore]
    public string ImageName => $"Manual_Cooking_C-{(int)(Center * 100)}_W-{(int)(Width * 100)}.png";
}
